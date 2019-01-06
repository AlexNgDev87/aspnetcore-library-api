using System;
using System.Collections;

namespace LibraryApi.Models
{
    // this Dto is for output
    public class AuthorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Genre { get; set; }
    }
}