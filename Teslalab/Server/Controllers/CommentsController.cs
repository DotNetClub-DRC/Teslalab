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
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService _commentsService;

        public CommentsController(ICommentsService commentsService)
        {
            _commentsService = commentsService;
        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<CommentDto>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<CommentDto>))]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CommentDto model)
        {
            var result = await _commentsService.CreateAsync(model);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<CommentDto>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<CommentDto>))]
        [HttpPut("Edit")]
        public async Task<IActionResult> Edit([FromBody] CommentDto model)
        {
            var result = await _commentsService.EditAsync(model);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<CommentDto>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<CommentDto>))]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _commentsService.RemoveAsync(id);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }
    }
}