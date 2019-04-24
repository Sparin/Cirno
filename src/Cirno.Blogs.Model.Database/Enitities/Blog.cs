using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Cirno.Blogs.Model.Enitities
{
    public class Blog
    {
        public long Id { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must have length between 3 and 50 symbols")]
        public string Name { get; set; }
        public long PostCount { get; set; } = 0;

        public List<Post> Posts { get; set; }
    }
}
