using Cirno.Blogs.Model.Enitities;
using Ganss.XSS;
using System;

namespace Cirno.Blogs.Security
{
    public static class PostExtensions
    {
        private static readonly string[] AllowedTags = { "h1", "h2", "h3", "h4", "p" };
        private static HtmlSanitizer Sanitizer = new HtmlSanitizer(AllowedTags);

        public static Post Sanitize(this Post post)
        {
            if (post == null)
                throw new ArgumentNullException("post");

            post.Title = new HtmlSanitizer().Sanitize(post.Title);
            post.Content = Sanitizer.Sanitize(post.Content);

            return post;
        }
    }
}






