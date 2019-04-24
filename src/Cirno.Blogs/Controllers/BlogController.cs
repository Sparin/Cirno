using AutoMapper;
using Cirno.Blogs.Model.DTO.Entities.Blog;
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
    public class BlogController : Controller
    {
        private readonly IBlogService _blogs;
        private readonly IMapper _mapper;
        private readonly ILogger<BlogController> _logger;

        public BlogController(IBlogService blogService, IMapper mapper, ILogger<BlogController> logger)
        {
            _blogs = blogService;
            _mapper = mapper;
            _logger = logger;
        }


        // GET: api/<controller>
        [HttpGet]
        public async Task<IActionResult> GetBlogsAsync(int page = Helpers.DEFAULT_PAGE, int limit = Helpers.MAX_LIMIT_ON_PAGE)
        {
            Helpers.CorrectPageLimitValues(ref page, ref limit);

            _logger.LogInformation($"User trying to get blogs on page {page} limit {limit}");
            var entities = await _blogs.GetBlogsAsync(page, limit);

            var result = _mapper.Map<IEnumerable<BlogDto>>(entities);
            _logger.LogInformation($"User received {entities.Count()} blogs");
            return Ok(result);
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogAsync(long id)
        {
            var entity = await _blogs.GetBlogAsync(id);

            _logger.LogInformation($"User trying to get blog with id {id}");
            if (entity == default(Blog))
            {
                _logger.LogWarning($"User requested not existing blog");
                return NotFound();
            }

            _logger.LogInformation($"User received blog with id {id}");
            var result = _mapper.Map<BlogDto>(entity);
            return Ok(result);
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBlogAsync([FromBody]CreateBlogDto requestDto)
        {
            var entity = _mapper.Map<Blog>(requestDto);

            Guid authorId;
            if (!Guid.TryParse(User.FindFirstValue("sub"), out authorId))
                throw new ArgumentException("Sub claim parsing failed");
            entity.AuthourId = authorId;

            _logger.LogInformation($"User (ID #{authorId}) trying to create new blog");
            entity.Sanitize();
            if (!TryValidateModel(entity))
            {
                _logger.LogWarning($"User's DTO does not pass model validation after sanitizing. {requestDto}");
                return BadRequest();
            }

            entity = await _blogs.CreateBlogAsync(entity);
            _logger.LogInformation($"User (ID #{authorId}) created new blog with identificator {entity.Id}");

            var result = _mapper.Map<BlogDto>(entity);
            return Ok(result);
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBlogAsync(long id, [FromBody]UpdateBlogDto requestDto)
        {
            Guid authorId;
            if (!Guid.TryParse(User.FindFirstValue("sub"), out authorId))
                throw new ArgumentException("Sub claim parsing failed");

            _logger.LogInformation($"User (ID #{authorId}) trying to update blog with identificator {id}");
            var entity = await _blogs.GetBlogAsync(id);

            if (entity == default(Blog))
            {
                _logger.LogWarning($"User (ID #{authorId}) requested not existing blog");
                return NotFound();
            }

            entity = _mapper.Map(requestDto, entity);

            if (authorId != entity.AuthourId)
            {
                _logger.LogWarning($"User (ID #{authorId}) tried to update not his own blog");
                return Forbid();
            }

            entity.Sanitize();
            if (!TryValidateModel(entity))
            {
                _logger.LogWarning($"User's DTO does not pass model validation after sanitizing. {requestDto}");
                return BadRequest();
            }

            entity = await _blogs.UpdateBlogAsync(entity);
            _logger.LogInformation($"User (ID #{authorId}) updated blog with identificator {entity.Id}");

            var result = _mapper.Map<BlogDto>(entity);
            return Ok(result);
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBlogAsync(int id)
        {
            Guid authorId;
            if (!Guid.TryParse(User.FindFirstValue("sub"), out authorId))
                throw new ArgumentException("Sub claim parsing failed");

            _logger.LogInformation($"User (ID #{authorId}) trying to delete blog with identificator {id}");
            var entity = await _blogs.GetBlogAsync(id);

            if (entity == default(Blog))
            {
                _logger.LogWarning($"User (ID #{authorId}) tried to delete not existing blog");
                return NotFound();
            }

            if (authorId != entity.AuthourId)
            {
                _logger.LogWarning($"User (ID #{authorId}) tried to delete not his own blog");
                return Forbid();
            }

            await _blogs.RemoveBlogAsync(entity);
            _logger.LogInformation($"Blog (ID #{entity.Id}) was delete by user (ID #{authorId})");

            return NoContent();
        }
    }
}
