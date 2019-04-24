using Cirno.Blogs.Model.Enitities;
using Ganss.XSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Cirno.Blogs.Security
{
    public static class BlogExtensions
    {
        private static readonly string[] AllowedTags = { };
        private static HtmlSanitizer Sanitizer = new HtmlSanitizer(AllowedTags);

        public static Blog Sanitize(this Blog blog)
        {
            if (blog == null)
                throw new ArgumentNullException("blog");

            blog.Name = Sanitizer.Sanitize(blog.Name);

            return blog;
        }
    }
}
