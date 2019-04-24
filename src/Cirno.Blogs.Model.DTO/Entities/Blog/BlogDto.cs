using Cirno.Blogs.Model.DTO.Entities.Post;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cirno.Blogs.Model.DTO.Entities.Blog
{
    public class BlogDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int PostCount { get; set; }
    }
}
