﻿using AutoMapper;
using Cirno.Blogs.Model.DTO.Entities.Blog;
using Cirno.Blogs.Model.Enitities;
using Cirno.Blogs.Security;
using Cirno.Blogs.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public BlogController(IBlogService blogService, IMapper mapper)
        {
            _blogs = blogService;
            _mapper = mapper;
        }


        // GET: api/<controller>
        [HttpGet]
        public async Task<IActionResult> GetBlogsAsync(int page = Helpers.DEFAULT_PAGE, int limit = Helpers.MAX_LIMIT_ON_PAGE)
        {
            Helpers.CorrectPageLimitValues(ref page, ref limit);
            var entities = await _blogs.GetBlogsAsync(page, limit);

            var result = _mapper.Map<IEnumerable<BlogDto>>(entities);
            return Ok(result);
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogAsync(long id)
        {
            var entity = await _blogs.GetBlogAsync(id);

            if (entity == default(Blog))
                return NotFound();

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

            entity.Sanitize();
            if (!TryValidateModel(entity))
                return BadRequest();

            entity = await _blogs.CreateBlogAsync(entity);

            var result = _mapper.Map<BlogDto>(entity);
            return Ok(result);
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBlogAsync(long id, [FromBody]UpdateBlogDto requestDto)
        {
            var entity = await _blogs.GetBlogAsync(id);

            if (entity == default(Blog))
                return NotFound();

            entity = _mapper.Map(requestDto, entity);

            Guid authorId;
            if (!Guid.TryParse(this.User.FindFirstValue("sub"), out authorId))
                throw new ArgumentException("Sub claim parsing failed");

            if (authorId != entity.AuthourId)
                return Forbid();

            entity.Sanitize();
            if (!TryValidateModel(entity))
                return BadRequest();

            entity = await _blogs.UpdateBlogAsync(entity);

            var result = _mapper.Map<BlogDto>(entity);
            return Ok(result);
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBlogAsync(int id)
        {
            var entity = await _blogs.GetBlogAsync(id);

            if (entity == default(Blog))
                return NotFound();

            Guid authorId;
            if (!Guid.TryParse(this.User.FindFirstValue("sub"), out authorId))
                throw new ArgumentException("Sub claim parsing failed");

            if (authorId != entity.AuthourId)
                return Forbid();

            await _blogs.RemoveBlogAsync(entity);

            return NoContent();
        }
    }
}
