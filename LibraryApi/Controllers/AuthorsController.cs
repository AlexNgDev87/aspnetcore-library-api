using AutoMapper;
using LibraryApi.Data.Models;
using LibraryApi.Data.Repository;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LibraryApi.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private ILibraryRepository _repository;

        public AuthorsController(ILibraryRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetAuthors()
        {
            var authorsFromRepo = _repository.GetAuthors();
            var authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);

            return Ok(authors);
        }

        [HttpGet("{id}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid id)
        {
            // try to avoid additional round trip
            var authorFromRepo = _repository.GetAuthor(id);

            if (authorFromRepo == null)
                return NotFound();

            var author = Mapper.Map<AuthorDto>(authorFromRepo);

            return Ok(author);
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] AuthorForCreationDto author)
        {
            if (author == null)
                return BadRequest();

            var authorEntity = Mapper.Map<Author>(author);
            _repository.AddAuthor(authorEntity);

            if (!_repository.Save())
            {
                // throw exception is a performance hit, it's expensive
                throw new Exception("Creating an author failed on save."); // For the moment, let the middleware handles it
                
                //return StatusCode(500, "A problem happened with handling your request.");
            }

            var authorToReturn = Mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthor", new { id = authorToReturn.Id }, authorToReturn);
        }
    }
}