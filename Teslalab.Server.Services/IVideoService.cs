using System.Threading.Tasks;
using Teslalab.Shared;
using Teslalab.Shared.DTOs;

namespace Teslalab.Server.Services
{
    public interface IVideoService
    {
        Task<OperationResponse<VideoDto>> CreateAsync(VideoDto model);

        Task<OperationResponse<VideoDto>> UpdateAsync(VideoDto model);

        Task<OperationResponse<VideoDto>> RemoveAsync(string id);

        Task<OperationResponse<VideoDto>> GetVideoDtoAsync(string videoId);

        CollectionResponse<VideoDto> GetAllVideos(string query, int pageNumber = 1, int pageSize = 10);
    }
}