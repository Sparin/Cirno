using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Cirno.Blogs.Model.Enitities
{
    public class Post
    {
        public long Id { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime UpdatedAt { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Title must have length between 3 and 50 symbols")]
        public string Title { get; set; }
        public string Content { get; set; }

        public long BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
