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

        [HttpDelete("{id}")]
        public IActionResult DeleteBookFromAuthor(Guid authorId, Guid id)
        {
            if (!_repository.AuthorExists(authorId))
                return NotFound();

            var bookForAuthorFromRepo = _repository.GetBookForAuthor(authorId, id);
            if (bookForAuthorFromRepo == null)
                return NotFound();

            _repository.DeleteBook(bookForAuthorFromRepo);

            if (!_repository.Save())
            {
                throw new Exception($"Deleting book {id} for author {authorId} failed on save.");
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        // Put has been used lessly these days, because it update the whole resource.
        // if an object has 30 fields, you need to send through the value of all 30 fields, or else it will be set to default value e.g. null
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid id, [FromBody] BookForUpdateDto book)
        {
            if (book == null)
                return BadRequest();

            if (_repository.AuthorExists(authorId))                                                                                           
                return NotFound();

            // Insert the book for author when the book doesn't exists
            var bookForAuthorFromRepo = _repository.GetBookForAuthor(authorId, id);
            if (bookForAuthorFromRepo == null)
            {
                var bookToAdd = Mapper.Map<Book>(book);
                bookToAdd.Id = id;

                _repository.AddBookForAuthor(authorId, bookToAdd);

                if (!_repository.Save())
                {
                    throw new Exception($"Upserting book {id} for author {authorId} failed on save.");
                }

                var bookToReturn = Mapper.Map<BookDto>(bookToAdd);

                return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, id = bookToAdd.Id }, bookToReturn);
            }
            
            // Mapper.Map update the value from dto to the context entity
            Mapper.Map(book, bookForAuthorFromRepo);

            // because Mapper.Map will update the value for us, therefore no implementation is needed in the update method
            _repository.UpdateBookForAuthor(bookForAuthorFromRepo);

            if (!_repository.Save())
            {
                throw new Exception($"Updating book {id} for author {authorId} failed on save.");
            }

            return NoContent();
        }
    }
}


