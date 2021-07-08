using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Teslalab.Server.Models.Models;

namespace Teslalab.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public UnitOfWork(UserManager<ApplicationUser> userManager,
                            RoleManager<IdentityRole> roleManager,
                            ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        private IUserRepository _user;

        public IUserRepository Users
        {
            get
            {
                if (_user == null)
                    _user = new IdentityUserRepository(_userManager, _roleManager);

                return _user;
            }
        }

        private IPlaylistRepository _playlist;

        public IPlaylistRepository Playlists
        {
            get
            {
                if (_playlist == null)
                    _playlist = new PlaylistRepository(_db);

                return _playlist;
            }
        }

        private IVideoRepository _video;

        public IVideoRepository Videos
        {
            get
            {
                if (_video == null)
                    _video = new VideoRepository(_db);

                return _video;
            }
        }

        private ICommentsRepository _comments;

        public ICommentsRepository Comments
        {
            get
            {
                if (_comments == null)
                    _comments = new CommentsRepository(_db);

                return _comments;
            }
        }

        public async Task CommitChangesAsync(string userId)
        {
            await _db.SaveChangesAsync(userId);
        }
    }
}