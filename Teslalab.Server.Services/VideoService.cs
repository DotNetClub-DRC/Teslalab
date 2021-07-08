using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using Teslalab.Repositories;
using Teslalab.Server.Infrastructure;
using Teslalab.Server.Models.Models;
using Teslalab.Server.Services.Utilities;
using Teslalab.Shared;
using Teslalab.Shared.DTOs;

namespace Teslalab.Server.Services
{
    public class VideoService : IVideoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IdentityOptions _identity;

        private readonly EnvironmentOptions _env;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _storage;
        private readonly ICommentsService _commentsService;

        public VideoService(IUnitOfWork unitOfWork,
                            IdentityOptions identity,
                            IFileStorageService storage,
                            EnvironmentOptions env,
                            IMapper mapper,
                            ICommentsService commentsService)
        {
            _unitOfWork = unitOfWork;
            _identity = identity;
            _storage = storage;
            _env = env;
            _mapper = mapper;
            _commentsService = commentsService;
        }

        public async Task<OperationResponse<VideoDto>> CreateAsync(VideoDto model)
        {
            // Validation
            var similarVideo = await _unitOfWork.Videos.GetByTitleAsync(model.Title);
            if (similarVideo != null)
                return new OperationResponse<VideoDto>
                {
                    IsSuccess = false,
                    Message = "Failed to create the video, another one has the same title"
                };

            if (model.ThumbFile == null)
                return new OperationResponse<VideoDto>
                {
                    IsSuccess = false,
                    Message = "Thump image is required"
                };

            string thumpUrl = string.Empty;

            try
            {
                thumpUrl = await _storage.SaveFileAsync(model.ThumbFile, "Uploads");
            }
            catch (BadImageFormatException)
            {
                return new OperationResponse<VideoDto>
                {
                    IsSuccess = false,
                    Message = "Please upload a valid image file"
                };
            }

            var video = new Video
            {
                Category = model.Category,
                Description = model.Description,
                Likes = 0,
                Views = 0,
                Privacy = model.Privacy,
                ThumpUrl = thumpUrl,
                Title = model.Title,
                VideoUrl = model.VideoUrl,
                PublishedDate = model.PublishedDate,
                Tags = model.Tags?.Select(t => new Tag
                {
                    Name = t,
                }).ToList(),
            };

            await _unitOfWork.Videos.CreateAsync(video);
            await _unitOfWork.CommitChangesAsync(_identity.UserId);

            model.Id = video.Id;
            model.ThumbFile = null;
            // TODO: Get the URL of the running API and replace the hardcoded one
            model.ThumpUrl = $"{_env.ApiUrl}/{thumpUrl}";

            return new OperationResponse<VideoDto>
            {
                IsSuccess = true,
                Message = "Video has been created successfully!",
                Data = model
            };
        }

        public CollectionResponse<VideoDto> GetAllVideos(string query, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 5)
                pageSize = 5;

            if (pageSize > 50)
                pageSize = 50;

            var videos = _unitOfWork.Videos.GetAll();
            int videosCount = videos.Count();

            var videosInPage = videos
                                 .Where(v => v.Title.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                                            || v.Description.Contains(query, StringComparison.InvariantCultureIgnoreCase))
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .Select(p => _mapper.Map<VideoDto>(p));

            int pagesCount = videosCount / pageSize;
            if ((videosCount % pageSize) != 0)
                pagesCount++;

            return new CollectionResponse<VideoDto>
            {
                IsSuccess = true,
                Message = "Videos retrieved successfully!",
                Records = videosInPage.ToArray(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                PagesCount = pagesCount
            };
        }

        public async Task<OperationResponse<VideoDto>> GetVideoDtoAsync(string videoId)
        {
            var video = await _unitOfWork.Videos.GetByIdAsync(videoId);
            if (video == null)
                return new OperationResponse<VideoDto>
                {
                    IsSuccess = false,
                    Message = "Video cannot be found"
                };

            var videoDto = _mapper.Map<VideoDto>(video);

            // Get all the comments of the video
            videoDto.Comments = _commentsService.GetVideoComments(videoId);

            return new OperationResponse<VideoDto>
            {
                IsSuccess = true,
                Data = videoDto,
                Message = "Video retrieved successfully!"
            };
        }

        public async Task<OperationResponse<VideoDto>> RemoveAsync(string id)
        {
            var video = await _unitOfWork.Videos.GetByIdAsync(id);

            if (video == null)
                return new OperationResponse<VideoDto>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Video not found"
                };
            _unitOfWork.Videos.RemoveTags(video);
            // TODO: Remove the comments and the playsist assingments
            _unitOfWork.Videos.Remove(video);
            _storage.RemoveFile(video.ThumpUrl);

            await _unitOfWork.CommitChangesAsync(_identity.UserId);

            return new OperationResponse<VideoDto>
            {
                IsSuccess = true,
                Message = "Video has been deleted successfully!",
                Data = _mapper.Map<VideoDto>(video),
            };
        }

        public async Task<OperationResponse<VideoDto>> UpdateAsync(VideoDto model)
        {
            var video = await _unitOfWork.Videos.GetByIdAsync(model.Id);

            if (video == null)
                return new OperationResponse<VideoDto>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Video not found"
                };

            // Check the thump image
            string thumpUrl = video.ThumpUrl;
            if (model.ThumbFile != null)
            {
                try
                {
                    thumpUrl = await _storage.SaveFileAsync(model.ThumbFile, "Uploads");
                    _storage.RemoveFile(video.ThumpUrl);
                }
                catch (BadImageFormatException)
                {
                    return new OperationResponse<VideoDto>
                    {
                        IsSuccess = false,
                        Message = "Please upload a valid image file"
                    };
                }
            }

            _unitOfWork.Videos.RemoveTags(video);

            video.Title = model.Title;
            video.Description = model.Description;
            video.Privacy = model.Privacy;
            video.Category = model.Category;
            video.PublishedDate = model.PublishedDate;
            video.ThumpUrl = thumpUrl;
            video.Tags = model.Tags?.Select(t => new Tag
            {
                Name = t,
            }).ToList();

            await _unitOfWork.CommitChangesAsync(_identity.UserId);

            model.ThumpUrl = $"{_env.ApiUrl}/{thumpUrl}";

            return new OperationResponse<VideoDto>
            {
                IsSuccess = true,
                Message = "Video has been updated successfully!",
                Data = model
            };
        }
    }
}