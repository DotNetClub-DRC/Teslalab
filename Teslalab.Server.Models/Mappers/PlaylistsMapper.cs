using System.Collections.Generic;
using Teslalab.Server.Models.Models;
using Teslalab.Shared.DTOs;

namespace Teslalab.Server.Models.Mappers
{
    public static class PlaylistsMapper
    {
        public static PlaylistDto ToPlaylistDto(this Playlist playlist, IEnumerable<VideoDto> playlistVideos = null, bool includeVideos = false)
        {
            return new PlaylistDto
            {
                Id = playlist.Id,
                Name = playlist.Name,
                Description = playlist.Description,
                Videos = includeVideos ? playlistVideos : null
            };
        }
    }
}