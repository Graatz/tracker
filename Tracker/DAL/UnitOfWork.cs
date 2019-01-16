using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tracker.Models;

namespace Tracker.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext context;
        private IRepository<Track> trackRepository;
        private IRepository<TrackPoint> trackPointRepository;
        private IRepository<UserConfig> userConfigRepository;
        private IRepository<ApplicationUser> userRepository;

        public ApplicationDbContext Context
        {
            get
            {
                if (context == null)
                    context = new ApplicationDbContext();

                return context;
            }
        }

        public IRepository<Track> TrackRepository
        {
            get
            {
                if (trackRepository == null)
                    this.trackRepository = new GenericRepository<Track>(Context);

                return trackRepository;
            }
        }

        public IRepository<TrackPoint> TrackPointRepository
        {
            get
            {
                if (trackPointRepository == null)
                    this.trackPointRepository = new GenericRepository<TrackPoint>(Context);

                return trackPointRepository;
            }
        }

        public IRepository<UserConfig> UserConfigRepository
        {
            get
            {
                if (userConfigRepository == null)
                    this.userConfigRepository = new GenericRepository<UserConfig>(Context);

                return userConfigRepository;
            }
        }

        public IRepository<ApplicationUser> UserRepository
        {
            get
            {
                if (userRepository == null)
                    this.userRepository = new GenericRepository<ApplicationUser>(Context);

                return userRepository;
            }
        }

        public void Save()
        {
            Context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}