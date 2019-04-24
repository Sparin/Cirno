using Cirno.Blogs.Model.Enitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cirno.Blogs.Services.Interfaces
{
    public interface IPostService
    {
        Task<Post> GetPostAsync(long postId);
        Task<IEnumerable<Post>> GetPostsAsync(int page, int limit, long? blogId = null);

        Task<Post> CreatePostAsync(Post post);
        Task<Post> UpdatePostAsync(Post post);
        Task RemovePostAsync(Post post);
    }
}
