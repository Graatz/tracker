using Microsoft.AspNet.Identity;
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
    public class StravaTrackService : IStravaTrackService
    {
        private UnitOfWork unitOfWork;
        private readonly string externalSignature = "Strava";

        public StravaTrackService(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Track GetTrackByExternalId(int externalId)
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

        public List<DetailedActivity> ImportActivities()
        {
            StravaClient stravaClient = new StravaClient();

            var activities = stravaClient.GetAthleteActivities();

            List<DetailedActivity> importedActivities = new List<DetailedActivity>();
            foreach (var activity in activities)
            {
                var track = GetTrackByExternalId(activity.Id);

                if (track != null)
                    continue;

                var detailedActivity = stravaClient.GetDetailedActivity(activity.Id, true);
                detailedActivity.Import = true;
                importedActivities.Add(detailedActivity);
            }

            return importedActivities;
        }

        public string Authorize(string code)
        {
            StravaClient stravaClient = new StravaClient();
            StravaAuthenticationResponse response = stravaClient.GetAccessToken(code);
            return response.AccessToken;
        }
    }
}