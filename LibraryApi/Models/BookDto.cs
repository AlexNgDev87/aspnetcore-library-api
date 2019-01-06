using System;
using System.Collections;

namespace LibraryApi.Models
{
    public class BookDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Guid AuthorId { get; set; }
    }
}