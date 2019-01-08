using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Models
{
    public class BookForUpdateDto
    {
        // id already comes from the Uri, to avoid duplicate id
        // CreationDto and UpdateDto stay the same, but the allow in update is not necessarily allow in creation
        public string Title { get; set; }

        public string Description { get; set; }
    }
}
