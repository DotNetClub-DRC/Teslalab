using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Teslalab.Server.Services;
using Teslalab.Shared;
using Teslalab.Shared.DTOs;

namespace Teslalab.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class VideosController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public VideosController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<VideoDto>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<VideoDto>))]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] VideoDto model)
        {
            var result = await _videoService.CreateAsync(model);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<VideoDto>))]
        [ProducesResponseType(404)]
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _videoService.GetVideoDtoAsync(id);
            if (!result.IsSuccess)
                return NotFound();
            return Ok(result);
        }

        [ProducesResponseType(200, Type = typeof(CollectionResponse<VideoDto>))]
        [AllowAnonymous]
        [HttpGet("GetAll")]
        public IActionResult GetAll(string query = "", int pageNumber = 1, int pageSize = 10)
        {
            var result = _videoService.GetAllVideos(query, pageNumber, pageSize);
            return Ok(result);
        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<VideoDto>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<VideoDto>))]
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] VideoDto model)
        {
            var result = await _videoService.UpdateAsync(model);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<VideoDto>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<VideoDto>))]
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