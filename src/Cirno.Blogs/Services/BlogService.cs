using Cirno.Blogs.Model.Database.Context;
using Cirno.Blogs.Model.Enitities;
using Cirno.Blogs.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Cirno.Blogs.Services
{
    public class BlogService : IBlogService
    {
        private readonly BloggingContext _dbContext;

        public BlogService(BloggingContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Blog> CreateBlogAsync(Blog blog)
        {
            if (blog == null)
                throw new ArgumentNullException("blog");

            _dbContext.Blogs.Add(blog);
            await _dbContext.SaveChangesAsync();

            return blog;
        }

        public async Task RemoveBlogAsync(Blog blog)
        {
            if (blog == null)
                throw new ArgumentNullException("blog");

            _dbContext.Blogs.Remove(blog);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Blog> GetBlogAsync(long blogId)
        {
            return await _dbContext.Blogs.FirstOrDefaultAsync(x => x.Id == blogId);
        }

        public async Task<IEnumerable<Blog>> GetBlogsAsync(int page, int limit)
        {
            if (page < 0)
                throw new ArgumentOutOfRangeException("page", "Page must be positive number");
            if (limit < 0)
                throw new ArgumentOutOfRangeException("limit", "Limit must be positive number");

            return await _dbContext.Blogs
                .OrderBy(x => x.Id)
                .Skip(page * limit)
                .Take(limit)
                .ToArrayAsync();
        }

        public async Task<Blog> UpdateBlogAsync(Blog blog)
        {
            if (blog == null)
                throw new ArgumentNullException("blog");

            _dbContext.Blogs.Update(blog);
            await _dbContext.SaveChangesAsync();

            return blog;
        }
    }
}
