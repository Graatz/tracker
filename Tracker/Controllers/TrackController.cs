using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tracker.Models;
using Microsoft.AspNet.Identity;
using Tracker.Helpers;
using System.Diagnostics;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using RestSharp;
using Tracker.Strava;
using Newtonsoft.Json;
using System.Text;

namespace Tracker.Controllers
{

    public class TrackController : Controller
    {
        protected ApplicationDbContext db = new ApplicationDbContext();

        public ViewResult Search()
        {
            var searchModel = new TrackSearchModel();

            return View(searchModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult FilterTracks(TrackSearchModel searchModel)
        {
            return RedirectToAction("Tracks", searchModel);
        }

        public ViewResult UploadForm()
        {
            var model = new Track();

            return View(model);
        }

        public ActionResult TracksTable()
        {
            var model = db.Tracks.Include(t => t.User).OrderByDescending(t => t.UploadDate).ToList();

            return View(model);
        }

        public ActionResult Tracks(TrackSearchModel searchModel)
        {
            var result = GetTracks(searchModel);

            var viewModel = new ListTracksViewModel()
            {
                NumberOfTracks = result.Count(),
                Tracks = result.Include(t => t.User).OrderByDescending(t => t.UploadDate).Take(10).ToList(),
                CurrentPage = 1,
                StartingIndex = 0,
                NumberOfTracksPerPage = 10
            };

            return View(viewModel);
        }
        
        public ActionResult TracksPage(int startingIndex, TrackSearchModel searchModel)
        {
            var result = GetTracks(searchModel);

            var viewModel = new ListTracksViewModel()
            {
                NumberOfTracks = result.Count(),
                Tracks = result.Include(t => t.User).OrderByDescending(t => t.UploadDate).Skip(startingIndex).Take(10).ToList(),
                CurrentPage = (startingIndex / 10) + 1,
                StartingIndex = startingIndex,
                NumberOfTracksPerPage = 10
            };

            return View("Tracks", viewModel);
        }

        public IQueryable<Track> GetTracks(TrackSearchModel searchModel)
        {
            var result = db.Tracks.AsQueryable();

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
                    var user = db.Users.SingleOrDefault(u => u.UserName.Equals(searchModel.User));
                    if (user != null)
                        result = result.Where(t => t.UserId == user.Id);
                }
            }

            return result;
        }

        public ViewResult Details(int id)
        {
            var track = db.Tracks.Include(t => t.User).SingleOrDefault(t => t.Id == id);
            var trackPoints = db.TrackPoints.Where(tp => tp.TrackId == id).ToList();

            // Getting tracks in min, max distance
            var similarTrackPoints = db.TrackPoints.Where(tp => tp.Latitude >= track.MinLatitude &&
                                                          tp.Latitude <= track.MaxLatitude &&
                                                          tp.Longitude >= track.MinLongitude &&
                                                          tp.Longitude <= track.MaxLongitude &&
                                                          tp.TrackId != track.Id).Include(tp => tp.Track).ToList();

            // Filtering tracks which are close enough to the route
            List<Track> filteredTracks = GeoLocation.GetTracksCloseToTrack(similarTrackPoints, trackPoints);

            var viewModel = new DetailsViewModel()
            {
                Track = track,
                TrackPoints = trackPoints,
                SimilarTracks = filteredTracks,
            };

            return View(viewModel);
        }

        public ViewResult Compare(int trackId1, int trackId2)
        {
            // Fetching tracks from database
            List<TrackPoint> trackPoints1 = db.TrackPoints.Where(tp => tp.TrackId == trackId1).OrderBy(tp => tp.Index).ToList();
            List<TrackPoint> trackPoints2 = db.TrackPoints.Where(tp => tp.TrackId == trackId2).OrderBy(tp => tp.Index).ToList();
            List<List<TrackPoint>> segmenty = GeoLocation.SplitToSegments(trackPoints2);

            // Filtering track2 points similar to track1
            List<TrackPoint> similarToTrack1 = GeoLocation.GetPointsCloseToTrack(trackPoints2, trackPoints1).OrderBy(tp => tp.Index).ToList();

            // Filtering track1 points similar to track2 filtered points
            List<TrackPoint> similarToFilteredTrack2 = GeoLocation.GetPointsCloseToTrack(trackPoints1, similarToTrack1).OrderBy(tp => tp.Index).ToList();

            List<List<TrackPoint>> trackSegments1 = new List<List<TrackPoint>>();
            trackSegments1.Add(db.TrackPoints.Where(tp => tp.TrackId == trackId1).OrderBy(tp => tp.Index).ToList());
            List<List<TrackPoint>> trackSegments2 = GeoLocation.SplitToSegments(similarToTrack1);

            var viewModel = new CompareViewModel()
            {
                Track1 = db.Tracks.SingleOrDefault(t => t.Id == trackId1),
                Track2 = db.Tracks.SingleOrDefault(t => t.Id == trackId2),
                Segments1 = trackSegments1,
                Segments2 = trackSegments2
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportStravaTracks(ImportConfirmationViewModel model)
        {
            StravaTrackHandler stravaTrackHandler = new StravaTrackHandler(db);

            foreach (var detailedActivity in model.DetailedActivities)
            {
                if (detailedActivity.Import)
                {
                    stravaTrackHandler.SetTrackData(detailedActivity);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(Track model, HttpPostedFileBase[] postedFiles)
        {
            if (!ModelState.IsValid || postedFiles.Length == 0)
                return View("UploadForm");

            DefaultTrackHandler defaultTrackHandler = new DefaultTrackHandler(db);
            GpxParser parser = new GpxParser();

            foreach (var postedFile in postedFiles)
            {
                if (postedFile == null)
                    return HttpNotFound();

                //string path = Server.MapPath("~/UploadedFiles/");

                List<TrackPoint> trkpts = parser.LoadGPXTracks(postedFile.InputStream);
                defaultTrackHandler.SetTrackData(model.Name + " (" + postedFile.FileName + ")", model.Description, trkpts);
            }

            return RedirectToAction("Index", "Home");
        }

        public ViewResult Authorization(string code)
        {
            if (code != null)
            {
                StravaClient StravaClient = new StravaClient();
                StravaAuthenticationResponse response = StravaClient.GetAccessToken(code);

                Session["access_token"] = response.AccessToken;

                var activities = StravaClient.GetAthleteActivities();

                List<DetailedActivity> importedActivities = new List<DetailedActivity>();
                foreach (var activity in activities)
                {
                    if (db.Tracks.SingleOrDefault(t => t.ExternalId == activity.Id) == null)
                    {
                        var detailedActivity = StravaClient.GetDetailedActivity(activity.Id, true);
                        detailedActivity.Import = true;
                        importedActivities.Add(detailedActivity);
                    }
                }

                var viewModel = new ImportConfirmationViewModel()
                {
                    DetailedActivities = importedActivities
                };

                return View("ImportConfirmation", viewModel);
            }

            return View();
        }

        public ActionResult Delete(int id)
        {
            var trackInDb = db.Tracks.SingleOrDefault(t => t.Id == id);

            if (trackInDb == null)
                return HttpNotFound();

            db.Tracks.Remove(trackInDb);
            db.SaveChanges();

            return RedirectToAction("TracksTable");
        }
    }
}