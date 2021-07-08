using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teslalab.Server.Models.Models;

namespace Teslalab.Repositories
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly ApplicationDbContext _db;

        public PlaylistRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(Playlist playlist)
        {
            await _db.Playlists.AddAsync(playlist);
        }

        public IEnumerable<Playlist> GetAll()
        {
            return _db.Playlists;
        }

        public async Task<Playlist> GetPlaylistById(string id)
        {
            return await _db.Playlists.FindAsync(id);
        }

        public void Remove(Playlist playlist)
        {
            _db.Playlists.Remove(playlist);
        }

        public IEnumerable<Video> GetAllVideosInPlaylist(string id)
        {
            return _db.PlaylistVideos
                        .Include(pv => pv.Video)
                        .Where(p => p.PlaylistId == id)
                        .Select(pv => pv.Video);
        }

        public void RemoveVideoFromPlaylist(PlaylistVideo playlistVideo)
        {
            _db.PlaylistVideos.Remove(playlistVideo);
        }

        public async Task AddVideoToPlaylistAsync(PlaylistVideo playlistVideo)
        {
            await _db.PlaylistVideos.AddAsync(playlistVideo);
        }

        public async Task<Playlist> GetByIdAsync(string id)
        {
            return await _db.Playlists
                            .Include(p => p.PlaylistVideos)
                            .ThenInclude(p => p.Video)
                            .SingleOrDefaultAsync(p => p.Id == id);
        }
    }
}