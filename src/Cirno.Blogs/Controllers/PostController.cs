using AutoMapper;
using Cirno.Blogs.Model.DTO.Entities.Post;
using Cirno.Blogs.Model.Enitities;
using Cirno.Blogs.Security;
using Cirno.Blogs.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly ILogger<PostController> _logger;

        public PostController(IPostService postService, IBlogService blogs, IMapper mapper, ILogger<PostController> logger)
        {
            _mapper = mapper;
            _posts = postService;
            _blogs = blogs;
            _logger = logger;
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<IActionResult> GetPostsAsync(long? blogId = null, int page = Helpers.DEFAULT_PAGE, int limit = Helpers.MAX_LIMIT_ON_PAGE)
        {
            Helpers.CorrectPageLimitValues(ref page, ref limit);

            _logger.LogInformation($"User trying to get posts on page {page} limit {limit}");
            var entities = await _posts.GetPostsAsync(page, limit, blogId);            

            var result = _mapper.Map<IEnumerable<PostDto>>(entities);
            _logger.LogInformation($"User received {entities.Count()} posts");
            return Ok(result);
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostAsync(long id)
        {
            var entity = await _posts.GetPostAsync(id);

            _logger.LogInformation($"User trying to get post with id {id}");
            if (entity == default(Post))
            {
                _logger.LogWarning($"User requested not existing post");
                return NotFound();
            }

            _logger.LogInformation($"User received post with id {id}");
            var result = _mapper.Map<PostDto>(entity);
            return Ok(result);
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePostAsync([FromBody]CreatePostDto requestDto)
        {
            Guid authorId;
            if (!Guid.TryParse(User.FindFirstValue("sub"), out authorId))
                throw new ArgumentException("Sub claim parsing failed");

            _logger.LogInformation($"User (ID #{authorId}) trying to create new post");
            var entity = _mapper.Map<Post>(requestDto);
            entity.AuthourId = authorId;


            _logger.LogInformation($"User (ID #{authorId}) trying to create new post");
            var blog = await _blogs.GetBlogAsync(entity.BlogId);
            if (blog == null)
            {
                _logger.LogWarning($"User (ID #{authorId}) tried to create new post in not existing blog");
                return BadRequest();
            }
        
            entity.Sanitize();
            if (!TryValidateModel(entity))
            {
                _logger.LogWarning($"User's DTO does not pass model validation after sanitizing. {requestDto}");
                return BadRequest();
            }

            entity = await _posts.CreatePostAsync(entity);
            _logger.LogInformation($"User (ID #{authorId}) created new post with identificator {entity.Id}");

            var result = _mapper.Map<PostDto>(entity);
            return Ok(result);
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePostAsync(long id, [FromBody]UpdatePostDto requestDto)
        {
            Guid authorId;
            if (!Guid.TryParse(User.FindFirstValue("sub"), out authorId))
                throw new ArgumentException("Sub claim parsing failed");

            _logger.LogInformation($"User (ID #{authorId}) trying to update post with identificator {id}");

            var entity = await _posts.GetPostAsync(id);
            if (entity == default(Post))
            {
                _logger.LogWarning($"User requested not existing post");
                return NotFound();
            }

            if (authorId != entity.AuthourId)
            {
                _logger.LogWarning($"User (ID #{authorId}) tried to update not his own post");
                return Forbid();
            }

            entity = _mapper.Map(requestDto, entity);
            entity.Sanitize();
            if (!TryValidateModel(entity))
            {
                _logger.LogWarning($"User's DTO does not pass model validation after sanitizing. {requestDto}");
                return BadRequest();
            }

            entity = await _posts.UpdatePostAsync(entity);
            _logger.LogInformation($"User (ID #{authorId}) updated post with identificator {entity.Id}");

            var result = _mapper.Map<PostDto>(entity);
            return Ok(result);
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePostAsync(long id)
        {
            Guid authorId;
            if (!Guid.TryParse(User.FindFirstValue("sub"), out authorId))
                throw new ArgumentException("Sub claim parsing failed");

            _logger.LogInformation($"User (ID #{authorId}) trying to delete post with identificator {id}");

            var entity = await _posts.GetPostAsync(id);
            if (entity == default(Post))
            {
                _logger.LogWarning($"User (ID #{authorId}) tried to delete not existing post");
                return NotFound();
            }

            if (authorId != entity.AuthourId)
            {
                _logger.LogWarning($"User (ID #{authorId}) tried to delete not his own post");
                return Forbid();
            }

            await _posts.RemovePostAsync(entity);
            _logger.LogInformation($"Post (ID #{entity.Id}) was delete by user (ID #{authorId})");

            return NoContent();
        }
    }
}
