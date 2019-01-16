using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tracker.Models;
using Microsoft.AspNet.Identity;
using Tracker.DAL;

namespace Tracker.Helpers
{
    public class TrackHandler
    {
        public string ExternalSignature { get; set; }

        public TrackHandler()
        {

        }

        public TrackHandler(string externalSignature)
        {
            ExternalSignature = externalSignature;
        }
        
        public Track SetTrackData(Track track, List<TrackPoint> trackPoints)
        {
            if (trackPoints.Count <= 1)
                return null;

            var userId = HttpContext.Current.User.Identity.GetUserId().ToString();

            track.ExternalSignature = ExternalSignature;
            track.UploadDate = DateTime.Now;
            track.TrackPoints = trackPoints;
            if (!SetData(track, trackPoints))
                return null;

            return track;
        }

        private bool SetData(Track track, List<TrackPoint> trackPoints)
        {
            if (trackPoints.Count <= 1)
                return false;

            for (int i = 0; i < trackPoints.Count; i++)
            {
                trackPoints[i].Track = track;
                trackPoints[i].Index = i;
            }

            track.MinLatitude = trackPoints.Min(tp => tp.Latitude);
            track.MinLongitude = trackPoints.Min(tp => tp.Longitude);
            track.MaxLatitude = trackPoints.Max(tp => tp.Latitude);
            track.MaxLongitude = trackPoints.Max(tp => tp.Longitude);
            double Distance = GeoMath.CalculateTrackDistance(trackPoints);
            double AvgSpeed = GeoMath.CalculateAvarageTrackSpeed(trackPoints);
            track.Distance = double.IsNaN(Distance) ? 0 : Distance;
            track.AvarageSpeed = double.IsNaN(AvgSpeed) ? 0 : AvgSpeed;
            track.StartLocation = GeoMath.ReversedGeoLocation(trackPoints[0].Latitude, trackPoints[0].Longitude);
            track.EndLocation = GeoMath.ReversedGeoLocation(trackPoints[trackPoints.Count - 1].Latitude, trackPoints[trackPoints.Count - 1].Longitude);

            var trackDate = trackPoints.Where(t => t.Date != null);
            if (trackDate.Count() > 0)
                track.TrackDate = trackDate.First().Date;

            return true;
        }
    }
}