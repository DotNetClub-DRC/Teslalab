using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Teslalab.Shared;
using Teslalab.Shared.DTOs;

namespace Teslalab.Server.Services
{
    public interface ICommentsService
    {
        Task<OperationResponse<CommentDto>> CreateAsync(CommentDto model);

        Task<OperationResponse<CommentDto>> EditAsync(CommentDto model);

        Task<OperationResponse<CommentDto>> RemoveAsync(string commentId);

        IEnumerable<CommentDto> GetVideoComments(string videoId);
    }
}