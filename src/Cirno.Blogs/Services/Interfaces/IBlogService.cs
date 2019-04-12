using Cirno.Blogs.Model.Enitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cirno.Blogs.Services.Interfaces
{
    public interface IBlogService
    {
        Task<Blog> GetBlogAsync(long blogId);
        Task<IEnumerable<Blog>> GetBlogsAsync(int page, int limit);

        Task<Blog> CreateBlogAsync(Blog blog);
        Task<Blog> UpdateBlogAsync(Blog blog);
        Task RemoveBlogAsync(Blog blog);
    }
}
