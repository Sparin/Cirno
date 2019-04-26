using Cirno.Blogs.Model.Database.Context;
using Cirno.Blogs.Model.Enitities;
using Cirno.Blogs.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cirno.Blogs.Services
{
    public class PostService : IPostService
    {
        private readonly BloggingContext _dbContext;

        public PostService(BloggingContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            if (post == null)
                throw new ArgumentNullException("post");

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                _dbContext.Posts.Add(post);
                if (post.Blog == null)
                    await _dbContext.Entry(post).Reference(x => x.Blog).LoadAsync();
                post.Blog.PostCount++;                

                await _dbContext.SaveChangesAsync();
                transaction.Commit();

                return post;
            }
        }

        public async Task RemovePostAsync(Post post)
        {
            if (post == null)
                throw new ArgumentNullException("post");

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                if (post.Blog == null)
                    await _dbContext.Entry(post).Reference(x => x.Blog).LoadAsync();

                post.Blog.PostCount--;
                _dbContext.Posts.Remove(post);

                await _dbContext.SaveChangesAsync();
                transaction.Commit();
            }
        }

        public async Task<Post> GetPostAsync(long postId)
        {
            return await _dbContext.Posts.SingleOrDefaultAsync(x => x.Id == postId);
        }

        public async Task<IEnumerable<Post>> GetPostsAsync(int page, int limit, long? blogId = null)
        {
            if (page < 0)
                throw new ArgumentOutOfRangeException("page", "Page must be positive number");
            if (limit < 0)
                throw new ArgumentOutOfRangeException("limit", "Limit must be positive number");

            if (blogId == null)
                return await _dbContext.Posts.OrderByDescending(x => x.CreatedAt).Skip(page * limit).Take(limit).ToArrayAsync();
            else
                return await _dbContext.Posts.OrderByDescending(x=>x.CreatedAt).Skip(page * limit).Where(x => x.BlogId == blogId).Take(limit).ToArrayAsync();
        }

        public async Task<Post> UpdatePostAsync(Post post)
        {
            if (post == null)
                throw new ArgumentNullException("post");

            post.UpdatedAt = DateTime.Now;
            _dbContext.Posts.Update(post);
            await _dbContext.SaveChangesAsync();

            return post;
        }
    }
}
