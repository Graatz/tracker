using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tracker.Models;
using Tracker.Strava;

namespace Tracker.Services
{
    public interface ITrackService
    {
        bool AddTrack(Track model, List<TrackPoint> trackPoints);
        bool AddTrack(DetailedActivity detailedActivity);
        List<Track> GetTracksSimilarToTrack(int trackId);
        Track GetTrackById(int trackId);
        void RemoveById(int trackId);
        Track GetTrackWithTrackPoints(int trackId);
        List<TrackPoint> GetTrackPointsByTrackId(int trackId);
        List<TrackPoint> GetTrackPointsByTrackIdInBounds(int trackId, double minLatitude, double maxLatitude, double minLongitude, double maxLongitude);
        List<TrackPoint> GetTrackPointsSimilarToTrack(int trackId1, int trackId2);
        IQueryable<Track> GetTracksFromSearchModel(TrackSearchModel searchModel);
        Track GetExternalTrack(long externalId, string externalSignature);
        void DuplicateTrack(int trackId, int number);
    }
}
