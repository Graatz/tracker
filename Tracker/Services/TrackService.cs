using Microsoft.AspNet.Identity;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tracker.DAL;
using Tracker.Helpers;
using Tracker.Models;
using Tracker.Strava;

namespace Tracker.Services
{
    public class TrackService : ITrackService
    {
        private IUnitOfWork unitOfWork;

        public TrackService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public bool AddTrack(Track model, List<TrackPoint> trackPoints)
        {
            var userId = HttpContext.Current.User.Identity.GetUserId().ToString();
            model.User = unitOfWork.UserRepository.GetByID(userId);

            TrackHandler defaultTrackHandler = new TrackHandler();

            Track trackDetails = defaultTrackHandler.SetTrackData(model, trackPoints);
            if (trackDetails == null)
                return false;

            unitOfWork.TrackRepository.Insert(trackDetails);
            unitOfWork.TrackPointRepository.InsertRange(trackDetails.TrackPoints.ToList());
            unitOfWork.Save();

            return true;
        }

        public Track GetTrackById(int trackId)
        {
            return unitOfWork.TrackRepository.Get(filter: t => t.Id == trackId, includeProperties: "User").SingleOrDefault();
        }

        public bool Intersects(Track track1, Track track2)
        {
            if (track1.MinLongitude <= track2.MaxLongitude &&
                track1.MaxLongitude >= track2.MinLongitude &&
                track1.MinLatitude <= track2.MaxLatitude &&
                track1.MaxLatitude >= track2.MinLatitude)
                return true;

            return false;
        }

        public List<Track> GetTracksSimilarToTrack(int trackId)
        {
            var track = unitOfWork.TrackRepository.Get(filter: t => t.Id == trackId, includeProperties: "User").SingleOrDefault();
            var trackPoints = unitOfWork.TrackPointRepository.Get(filter: tp => tp.TrackId == trackId).ToList();

            var userId = HttpContext.Current.User.Identity.GetUserId();
            var userConfig = unitOfWork.UserConfigRepository.Get(filter: c => c.UserId == userId).SingleOrDefault();

            var margin = Math.Pow(userConfig.SearchingDistance, 2);

            var similarTrackPoints = new List<TrackPoint>();

            similarTrackPoints = unitOfWork.TrackPointRepository.Get(
                filter: tp => tp.Latitude >= track.MinLatitude - margin &&
                tp.Latitude <= track.MaxLatitude + margin &&
                tp.Longitude >= track.MinLongitude - margin &&
                tp.Longitude <= track.MaxLongitude + margin &&
                tp.TrackId != track.Id,
                includeProperties: "Track").ToList();

            /*var similarTrackPoints = new List<TrackPoint>();
            similarTrackPoints = unitOfWork.TrackRepository.Get(
            filter: tp =>
                tp.Id != track.Id &&
                tp.MinLongitude <= track.MaxLongitude &&
                tp.MaxLongitude >= track.MinLongitude &&
                tp.MinLatitude <= track.MaxLatitude &&
                tp.MaxLatitude >= track.MinLatitude,
            orderBy: null,
            includeProperties: "TrackPoints").SelectMany(t => t.TrackPoints).ToList();*/
            // Filtering tracks which are close enough to the route
            List<Track> filteredTracks = GeoMath.GetTracksCloseToTrack(
                similarTrackPoints,
                trackPoints,
                userConfig.SearchingDistance,
                new TimeSpan(userConfig.TimeSpanDays, userConfig.TimeSpanHours, userConfig.TimeSpanMinutes, 0)
            );

            return filteredTracks;
        }

        public Track GetTrackWithTrackPoints(int trackId)
        {
            var track = unitOfWork.TrackRepository.Get(filter: tp => tp.Id == trackId, includeProperties: "TrackPoints").FirstOrDefault();

            return track;
        }

        public List<TrackPoint> GetTrackPointsByTrackId(int trackId)
        {
            return unitOfWork.TrackPointRepository.Get(filter: tp => tp.TrackId == trackId).ToList();
        }

        public List<TrackPoint> GetTrackPointsByTrackIdInBounds(int trackId, double minLatitude, double maxLatitude, double minLongitude, double maxLongitude)
        {
            var trackPoints = unitOfWork.TrackPointRepository.Get(
                filter: tp => tp.TrackId == trackId &&
                tp.Latitude >= minLatitude &&
                tp.Latitude <= maxLatitude &&
                tp.Longitude >= minLongitude &&
                tp.Longitude <= maxLongitude,
                orderBy: tp => tp.OrderBy(t => t.Index),
                includeProperties: "Track").ToList();

            return trackPoints;
        }

        public List<TrackPoint> GetTrackPointsSimilarToTrack(int trackId1, int trackId2)
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var userConfig = unitOfWork.UserConfigRepository.Get(filter: c => c.UserId == userId).SingleOrDefault();
            var track1 = GetTrackById(trackId1);

            var trackPoints1 = GetTrackPointsByTrackId(trackId1);
            var trackPoints2 = GetTrackPointsByTrackIdInBounds(trackId2, track1.MinLatitude, track1.MaxLatitude, track1.MinLongitude, track1.MaxLongitude);

            var similarToTrack1 = GeoMath.GetPointsCloseToTrack(
                trackPoints2,
                trackPoints1,
                userConfig.SearchingDistance,
                new TimeSpan(userConfig.TimeSpanDays, userConfig.TimeSpanHours, userConfig.TimeSpanMinutes, 0)
            ).OrderBy(tp => tp.Index).ToList();

            return similarToTrack1;
        }

        public IQueryable<Track> GetTracksFromSearchModel(TrackSearchModel searchModel)
        {
            var result = unitOfWork.TrackRepository.Get().AsQueryable();

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.Name))
                    result = result.Where(t => t.Name.Contains(searchModel.Name));
                if (searchModel.Date.HasValue)
                    result = result.Where(t => t.TrackDate.Equals(searchModel.Date));
                if (!string.IsNullOrEmpty(searchModel.Location))
                    result = result.Where(t => t.StartLocation.Contains(searchModel.Location) || t.EndLocation.Contains(searchModel.Location));
                if (!string.IsNullOrEmpty(searchModel.User))
                {
                    var user = unitOfWork.UserRepository.Get(filter: u => u.UserName.Equals(searchModel.User)).SingleOrDefault();
                    if (user != null)
                        result = result.Where(t => t.UserId == user.Id);
                }
            }

            return result;
        }

        public void RemoveById(int trackId)
        {
            unitOfWork.TrackRepository.Delete(trackId);
            unitOfWork.Save();
        }

        public Track GetExternalTrack(long externalId, string externalSignature)
        {
            var track = unitOfWork.TrackRepository.Get(
                    filter: t => t.ExternalId == externalId &&
                    t.ExternalSignature.Equals(externalSignature)).SingleOrDefault();

            return track;
        }

        public bool AddTrack(DetailedActivity detailedActivity)
        {
            var userId = HttpContext.Current.User.Identity.GetUserId().ToString();

            Track track = new Track()
            {
                Name = detailedActivity.Name,
                Description = detailedActivity.Description,
                TrackDate = detailedActivity.Date,
                ExternalId = detailedActivity.Id,
                User = unitOfWork.UserRepository.GetByID(userId)
            };

            var trackPoints = GeoMath.DecodePolylinePoints(detailedActivity.Map.Polyline);

            TrackHandler defaultTrackHandler = new TrackHandler();

            Track trackDetails = defaultTrackHandler.SetTrackData(track, trackPoints);
            if (trackDetails == null)
                return false;

            unitOfWork.TrackRepository.Insert(trackDetails);
            unitOfWork.TrackPointRepository.InsertRange(trackDetails.TrackPoints.ToList());
            unitOfWork.Save();

            return true;
        }

        public void DuplicateTrack(int trackId, int number)
        {
            for (int i = 0; i < number; i++)
            {
                var track = unitOfWork.TrackRepository.Get(filter: t => t.Id == trackId, orderBy: null, includeProperties: "TrackPoints, User").FirstOrDefault();

                AddTrack(track, track.TrackPoints.ToList());
            }
        }
    }
}