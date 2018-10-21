using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tracker.Models;
using Microsoft.AspNet.Identity;

namespace Tracker.Helpers
{
    public class DefaultTrackHandler : TrackHandler
    {     
        public DefaultTrackHandler(ApplicationDbContext db) : base(db)
        {

        }

        public bool SetTrackData(string name, string description, List<TrackPoint> trackPoints)
        {
            if (trackPoints.Count <= 1)
                return false;

            var userName = HttpContext.Current.User.Identity.GetUserName();
            var userId = HttpContext.Current.User.Identity.GetUserId().ToString();

            Track track = new Track()
            {
                Name = name,
                Description = description,
                User = db.Users.SingleOrDefault(u => u.Id == userId),
                UploadDate = DateTime.Now
            };

            for (int i = 0; i < trackPoints.Count; i++)
            {
                trackPoints[i].Track = track;
                trackPoints[i].Index = i;
            }

            track.MinLatitude = trackPoints.Min(tp => tp.Latitude);
            track.MinLongitude = trackPoints.Min(tp => tp.Longitude);
            track.MaxLatitude = trackPoints.Max(tp => tp.Latitude);
            track.MaxLongitude = trackPoints.Max(tp => tp.Longitude);
            track.Distance = GeoLocation.CalculateTrackDistance(trackPoints);
            track.AvarageSpeed = GeoLocation.CalculateAvarageTrackSpeed(trackPoints);
            track.StartLocation = GeoLocation.ReversedGeoLocation(trackPoints[0].Latitude, trackPoints[0].Longitude);
            track.EndLocation = GeoLocation.ReversedGeoLocation(trackPoints[trackPoints.Count - 1].Latitude, trackPoints[trackPoints.Count - 1].Longitude);
            track.TrackDate = trackPoints[0].Date;

            AddTrackToDb(track, trackPoints);

            return true;
        }
    }
}