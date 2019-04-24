using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Cirno.Blogs.Model.Enitities
{
    public class Blog
    {
        public long Id { get; set; }

        public string Name { get; set; }
        public long PostCount { get; set; } = 0;

        public List<Post> Posts { get; set; }
    }
}
