using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Teslalab.Repositories;
using Teslalab.Server.Infrastructure;
using Teslalab.Shared;
using Teslalab.Server.Models.Models;
using Teslalab.Shared.DTOs;
using System.Linq;
using Teslalab.Server.Models.Mappers;

namespace Teslalab.Server.Services
{
    public class CommentsService : ICommentsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IdentityOptions _identity;

        public CommentsService(IUnitOfWork unitOfWork,
                               IdentityOptions identity)
        {
            _unitOfWork = unitOfWork;
            _identity = identity;
        }

        public async Task<OperationResponse<CommentDto>> CreateAsync(CommentDto model)
        {
            // check for the parent comment if the comment is reply
            Comment parentComment = null;
            if (!string.IsNullOrWhiteSpace(model.ParentCommentId))
            {
                parentComment = await _unitOfWork.Comments.GetByIdAsync(model.ParentCommentId);
                if (parentComment == null)
                    return new OperationResponse<CommentDto>
                    {
                        IsSuccess = false,
                        Message = "Main comment cannot be found"
                    };
            }

            // Check for the video
            if (string.IsNullOrWhiteSpace(model.VideoId))
                return new OperationResponse<CommentDto> { IsSuccess = false, Message = "Video is required" };

            var video = await _unitOfWork.Videos.GetByIdAsync(model.VideoId);
            if (video == null)
            {
                return new OperationResponse<CommentDto>
                {
                    IsSuccess = false,
                    Message = "Video cannot be found"
                };
            }

            var newComment = new Comment
            {
                Content = model.Content,
                Likes = 0,
                ParentComment = parentComment,
                Video = video,
            };

            await _unitOfWork.Comments.CreateAsync(newComment);
            await _unitOfWork.CommitChangesAsync(_identity.UserId);
            model.Id = newComment.Id;
            return new OperationResponse<CommentDto>
            {
                IsSuccess = true,
                Message = "Comment submited successfully",
                Data = model
            };
        }

        public async Task<OperationResponse<CommentDto>> EditAsync(CommentDto model)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(model.Id);
            if (comment == null)
                return new OperationResponse<CommentDto>
                {
                    IsSuccess = false,
                    Message = "Comment not found"
                };

            comment.Content = model.Content;

            await _unitOfWork.CommitChangesAsync(_identity.UserId);

            return new OperationResponse<CommentDto>
            {
                IsSuccess = true,
                Data = model,
                Message = "Comment has been edited successfully!"
            };
        }

        public IEnumerable<CommentDto> GetVideoComments(string videoId)
        {
            var comments = _unitOfWork.Comments.GetAllForVideo(videoId).ToArray();

            return comments.Select(c => c.ToCommentDto());
        }

        public async Task<OperationResponse<CommentDto>> RemoveAsync(string commentId)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
            if (comment == null)
                return new OperationResponse<CommentDto>
                {
                    IsSuccess = false,
                    Message = "Comment not found"
                };

            _unitOfWork.Comments.Remove(comment);
            await _unitOfWork.CommitChangesAsync(_identity.UserId);

            return new OperationResponse<CommentDto>
            {
                Data = comment.ToCommentDto(),
                IsSuccess = true,
                Message = "Comment deleted successfully"
            };
        }
    }
}