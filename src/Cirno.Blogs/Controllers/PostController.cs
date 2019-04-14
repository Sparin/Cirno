using AutoMapper;
using Cirno.Blogs.Model.DTO.Entities.Post;
using Cirno.Blogs.Model.Enitities;
using Cirno.Blogs.Security;
using Cirno.Blogs.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cirno.Blogs.Controllers
{
    [Route("api/[controller]")]
    public class PostController : Controller
    {
        private readonly IPostService _posts;
        private readonly IMapper _mapper;
        private readonly IBlogService _blogs;

        public PostController(IPostService postService, IBlogService blogs, IMapper mapper)
        {
            _mapper = mapper;
            _posts = postService;
            _blogs = blogs;
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<IActionResult> GetPostsAsync(long? blogId = null, int page = Helpers.DEFAULT_PAGE, int limit = Helpers.MAX_LIMIT_ON_PAGE)
        {
            Helpers.CorrectPageLimitValues(ref page, ref limit);
            var entities = await _posts.GetPostsAsync(page, limit, blogId);

            var result = _mapper.Map<IEnumerable<PostDto>>(entities);
            return Ok(result);
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostAsync(long id)
        {
            var entity = await _posts.GetPostAsync(id);

            if (entity == default(Post))
                return NotFound();

            var result = _mapper.Map<PostDto>(entity);
            return Ok(result);
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> CreatePostAsync([FromBody]CreatePostDto requestDto)
        {            
            var entity = _mapper.Map<Post>(requestDto);
            var blog = await _blogs.GetBlogAsync(entity.BlogId);
            if (blog == null)
                return BadRequest();

            entity.Sanitize();
            if (!TryValidateModel(entity))
                return BadRequest();

            entity = await _posts.CreatePostAsync(entity);

            var result = _mapper.Map<PostDto>(entity);
            return Ok(result);
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePostAsync(long id, [FromBody]UpdatePostDto requestDto)
        {
            var entity = await _posts.GetPostAsync(id);

            if (entity == default(Post))
                return NotFound();

            entity = _mapper.Map(requestDto, entity);
            entity.Sanitize();
            if (!TryValidateModel(entity))
                return BadRequest();

            entity = await _posts.UpdatePostAsync(entity);

            var result = _mapper.Map<PostDto>(entity);
            return Ok(result);
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePostAsync(int id)
        {
            var entity = await _posts.GetPostAsync(id);

            if (entity == default(Post))
                return NotFound();

            await _posts.RemovePostAsync(entity);

            return NoContent();
        }
    }
}
