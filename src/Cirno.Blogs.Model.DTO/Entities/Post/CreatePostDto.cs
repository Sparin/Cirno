using System;
using System.Collections.Generic;
using System.Text;

namespace Cirno.Blogs.Model.DTO.Entities.Post
{
    public class CreatePostDto
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public long BlogId { get; set; }
    }
}
