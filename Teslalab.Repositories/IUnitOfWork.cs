using System.Threading.Tasks;

namespace Teslalab.Repositories
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IPlaylistRepository Playlists { get; }
        IVideoRepository Videos { get; }
        ICommentsRepository Comments { get; }

        Task CommitChangesAsync(string userId);
    }
}