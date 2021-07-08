using System.Threading.Tasks;
using Teslalab.Shared;
using Teslalab.Shared.DTOs;

namespace Teslalab.Server.Services
{
    public interface IPlaylistService
    {
        Task<OperationResponse<PlaylistDto>> CreateAsync(PlaylistDto model);

        Task<OperationResponse<PlaylistDto>> UpdateAsync(PlaylistDto model);

        Task<OperationResponse<PlaylistDto>> RemoveAsync(string id);

        CollectionResponse<PlaylistDto> GetAllPlaylists(int pageNumber = 1, int pageSize = 10);

        Task<OperationResponse<VideoDto>> AssignOrRemoveVideoFromPlaylistAsync(PlaylistVideoRequest request);

        Task<OperationResponse<PlaylistDto>> GetSinglePlaylistAsync(string id);
    }
}