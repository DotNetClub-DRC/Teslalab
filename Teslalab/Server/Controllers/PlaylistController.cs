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
    public class PlaylistController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<PlaylistDto>))]
        [ProducesResponseType(404)]
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _playlistService.GetSinglePlaylistAsync(id);
            if (result.IsSuccess)
                return Ok(result);

            return NotFound();
        }

        [ProducesResponseType(200, Type = typeof(CollectionResponse<PlaylistDto>))]
        [AllowAnonymous]
        [HttpGet("GetAll")]
        public IActionResult GetAll(int pageNumber, int pageSize)
        {
            var result = _playlistService.GetAllPlaylists(pageNumber, pageSize);
            return Ok(result);
        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<PlaylistDto>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<PlaylistDto>))]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(PlaylistDto model)
        {
            var result = await _playlistService.CreateAsync(model);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<PlaylistDto>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<PlaylistDto>))]
        [HttpPut("Update")]
        public async Task<IActionResult> Update(PlaylistDto model)
        {
            var result = await _playlistService.UpdateAsync(model);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<PlaylistDto>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<PlaylistDto>))]
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _playlistService.RemoveAsync(id);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<string>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<string>))]
        [HttpPost("AssignOrRemoveVideo")]
        public async Task<IActionResult> AssignOrRemoveVideo([FromBody] PlaylistVideoRequest model)
        {
            var result = await _playlistService.AssignOrRemoveVideoFromPlaylistAsync(model);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }
    }
}