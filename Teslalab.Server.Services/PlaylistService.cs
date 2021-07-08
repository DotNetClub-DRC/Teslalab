using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teslalab.Repositories;
using Teslalab.Server.Infrastructure;
using Teslalab.Server.Models.Mappers;
using Teslalab.Server.Models.Models;
using Teslalab.Shared;
using Teslalab.Shared.DTOs;

namespace Teslalab.Server.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IdentityOptions _identity;
        private readonly IMapper _mapper;

        public PlaylistService(IUnitOfWork unitOfWork, IdentityOptions identity, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _identity = identity;
            _mapper = mapper;
        }

        public async Task<OperationResponse<VideoDto>> AssignOrRemoveVideoFromPlaylistAsync(PlaylistVideoRequest request)
        {
            var video = await _unitOfWork.Videos.GetByIdAsync(request.VideoId);
            if (video == null)
                return new OperationResponse<VideoDto>
                {
                    IsSuccess = false,
                    Message = "Video Cannot be found"
                };

            var playlist = await _unitOfWork.Playlists.GetByIdAsync(request.PlaylistId);
            if (playlist == null)
                return new OperationResponse<VideoDto>
                {
                    IsSuccess = false,
                    Message = "Playlist Cannot be found"
                };

            var playlistVideo = playlist.PlaylistVideos.SingleOrDefault(pv => pv.VideoId == request.VideoId);

            string message = string.Empty;
            // Check if the video is already in the playlist
            if (playlistVideo != null)
            {
                // Remove the video from the playlist
                _unitOfWork.Playlists.RemoveVideoFromPlaylist(playlistVideo);

                message = "Video has been removed from the playlist";
            }
            else
            {
                // Add the video the playlist
                await _unitOfWork.Playlists.AddVideoToPlaylistAsync(new PlaylistVideo
                {
                    Playlist = playlist,
                    Video = video
                });

                message = "Video has been added from the playlist";
            }
            await _unitOfWork.CommitChangesAsync(_identity.UserId);

            return new OperationResponse<VideoDto>
            {
                IsSuccess = true,
                Message = message
            };
        }

        public async Task<OperationResponse<PlaylistDto>> CreateAsync(PlaylistDto model)
        {
            var playlist = new Playlist
            {
                Name = model.Name,
                Description = model.Description
            };

            await _unitOfWork.Playlists.CreateAsync(playlist);
            await _unitOfWork.CommitChangesAsync(_identity.UserId);

            model.Id = playlist.Id;

            return new OperationResponse<PlaylistDto>
            {
                IsSuccess = true,
                Message = "Playlist create Successfully",
                Data = model
            };
        }

        public CollectionResponse<PlaylistDto> GetAllPlaylists(int pageNumber = 1, int pageSize = 10)
        {
            //Validation

            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 5)
                pageSize = 5;

            if (pageSize > 50)
                pageSize = 50;

            var playlists = _unitOfWork.Playlists.GetAll();

            int playlistCount = playlists.Count();

            var playlistsInRange = playlists
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .Select(p => p.ToPlaylistDto());

            int pagesCount = playlistCount / pageSize;
            if ((playlistCount & pageSize) != 0)
                pagesCount++;

            return new CollectionResponse<PlaylistDto>
            {
                IsSuccess = true,
                Message = "Playlists retrieved successfully!",
                Records = playlistsInRange.ToArray(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                PagesCount = pagesCount
            };
        }

        public async Task<OperationResponse<PlaylistDto>> GetSinglePlaylistAsync(string id)
        {
            var playlist = await _unitOfWork.Playlists.GetByIdAsync(id);
            if (playlist == null)
                return new OperationResponse<PlaylistDto> { IsSuccess = false, Message = "Playlist cannot be found" };

            var videos = playlist.PlaylistVideos?.Select(pv => _mapper.Map<VideoDto>(pv.Video)).ToArray();

            return new OperationResponse<PlaylistDto>
            {
                Data = playlist.ToPlaylistDto(videos, true),
                Message = "Playlist has been retrieved successfully!",
                IsSuccess = true
            };
        }

        public async Task<OperationResponse<PlaylistDto>> RemoveAsync(string id)
        {
            var playlist = await _unitOfWork.Playlists.GetPlaylistById(id);

            if (playlist == null)
                return new OperationResponse<PlaylistDto>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Playlist not found"
                };

            _unitOfWork.Playlists.Remove(playlist);

            await _unitOfWork.CommitChangesAsync(_identity.UserId);

            return new OperationResponse<PlaylistDto>
            {
                IsSuccess = true,
                Message = "Playlist has been deleted successfully!",
                Data = playlist.ToPlaylistDto()
            };
        }

        public async Task<OperationResponse<PlaylistDto>> UpdateAsync(PlaylistDto model)
        {
            var playlist = await _unitOfWork.Playlists.GetPlaylistById(model.Id);

            if (playlist == null)
                return new OperationResponse<PlaylistDto>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Playlist not found"
                };
            playlist.Name = model.Name;
            playlist.Description = model.Description;

            await _unitOfWork.CommitChangesAsync(_identity.UserId);

            return new OperationResponse<PlaylistDto>
            {
                IsSuccess = true,
                Message = "Playlist has been updated successfully!",
                Data = model,
            };
        }
    }
}