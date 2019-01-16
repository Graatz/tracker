using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracker.Models;

namespace Tracker.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        ApplicationDbContext Context { get; }
        IRepository<Track> TrackRepository { get; }
        IRepository<TrackPoint> TrackPointRepository { get; }
        IRepository<UserConfig> UserConfigRepository { get; }
        IRepository<ApplicationUser> UserRepository { get; }

        void Save();
    }
}
