using System;
using System.Collections.Generic;
using System.Text;

namespace Cirno.Blogs.Model.DTO.Entities.Post
{
    public class PostDto
    {
        public long Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public long BlogId { get; set; }
    }
}
