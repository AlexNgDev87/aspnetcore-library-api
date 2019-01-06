using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LibraryApi.Data.Models;
using LibraryApi.Data.Repository;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : ControllerBase
    {
        private ILibraryRepository _repository;

        public BooksController(ILibraryRepository repository)
        {
            _repository = repository;
        }

        [HttpGet()]
        public IActionResult GetBooksFromAuthor(Guid authorId)
        {
            if (!_repository.AuthorExists(authorId))
                return NotFound();

            var booksAuthorFromRepo = _repository.GetBooksForAuthor(authorId);
            var booksForAuthor = Mapper.Map<IEnumerable<BookDto>>(booksAuthorFromRepo);

            return Ok(booksForAuthor);
        }

        [HttpGet("{id}", Name = "GetBookForAuthor")]
        public IActionResult GetBookForAuthor(Guid authorId, Guid id)
        {
            if (!_repository.AuthorExists(authorId))
                return NotFound();

            var bookForAuthorFromRepo = _repository.GetBookForAuthor(authorId, id);
            if (bookForAuthorFromRepo == null)
                return NotFound();

            var bookForAuthor = Mapper.Map<BookDto>(bookForAuthorFromRepo);
            return Ok(bookForAuthor);
        }

        [HttpPost]
        public IActionResult CreateBookForAuthor(Guid authorId, [FromBody] BookForCreationDto book)
        {
            if (book == null)
                return BadRequest();

            if (!_repository.AuthorExists(authorId))
                return NotFound();

            var bookEntity = Mapper.Map<Book>(book);
            _repository.AddBookForAuthor(authorId, bookEntity);

            if (!_repository.Save())
            {
                throw new Exception($"Creating a book for author {authorId} failed on save.");
            }

            var bookToReturn = Mapper.Map<BookDto>(bookEntity);

            return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, id = bookToReturn.Id }, bookToReturn);
        }
    }
}


