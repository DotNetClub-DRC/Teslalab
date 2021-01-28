﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teslalab.Server.Services;
using Teslalab.Shared.DTOs;

namespace Teslalab.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public VideosController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] VideoDto model)
        {
            var result = await _videoService.CreateAsync(model);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAll(string id)
        {
            var result = await _videoService.GetVideoDtoAsync(id);
            if (!result.IsSuccess)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll(string query = "", int pageNumber = 1, int pageSize = 10)
        {
            var result = _videoService.GetAllVideos(query, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] VideoDto model)
        {
            var result = await _videoService.UpdateAsync(model);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _videoService.RemoveAsync(id);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }
    }
}