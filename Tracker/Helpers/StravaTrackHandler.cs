using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tracker.Models;
using Microsoft.AspNet.Identity;
using Tracker.Strava;

namespace Tracker.Helpers
{
    public class StravaTrackHandler : TrackHandler
    {
        public StravaTrackHandler(ApplicationDbContext db) : base(db)
        {

        }

        public bool SetTrackData(DetailedActivity detailedActivity)
        {
            var trackPoints = GeoMath.DecodePolylinePoints(detailedActivity.Map.Polyline);

            if (trackPoints.Count <= 1)
                return false;

            var userId = HttpContext.Current.User.Identity.GetUserId().ToString();

            Track track = new Track()
            {
                Name = detailedActivity.Name + "(STRAVA)",
                ExternalId = detailedActivity.Id,
                Description = detailedActivity.Description,
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
            track.Distance = (float)(detailedActivity.Distance / 1000);
            track.AvarageSpeed = detailedActivity.AverageSpeed * 3.6f;
            track.StartLocation = GeoMath.ReversedGeoLocation(trackPoints[0].Latitude, trackPoints[0].Longitude);
            track.EndLocation = GeoMath.ReversedGeoLocation(trackPoints[trackPoints.Count - 1].Latitude, trackPoints[trackPoints.Count - 1].Longitude);
            track.TrackDate = trackPoints[0].Date;

            AddTrackToDb(track, trackPoints);
            return true;
        }
    }
}