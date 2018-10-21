using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tracker.Models;
using Microsoft.AspNet.Identity;

namespace Tracker.Helpers
{
    public abstract class TrackHandler
    {
        protected ApplicationDbContext db;

        public TrackHandler(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void AddTrackToDb(Track track, List<TrackPoint> trackPoints)
        {
            db.Tracks.Add(track);
            db.TrackPoints.AddRange(trackPoints);
            db.SaveChanges();
        }
    }
}