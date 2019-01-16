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
using System.Data.Entity;
using Tracker.Strava;
using Tracker.DAL;
using Tracker.Services;
using Tracker.DTO;
using AutoMapper;

namespace Tracker.Controllers
{
    public class TrackController : Controller
    {
        private ITrackService trackService;

        public TrackController(ITrackService trackService)
        {
            this.trackService = trackService;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            trackService.RemoveById(id);
            
            return RedirectToAction("TracksTable", "Admin");
        }

        public ViewResult Search()
        {
            var searchModel = new TrackSearchModel();

            return View(searchModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult FilterTracks(int startingIndex = 0, TrackSearchModel searchModel = null)
        {
            return RedirectToAction("ListTracks", new
            {
                startingIndex,
                searchModel.Date,
                searchModel.Name,
                searchModel.Location,
                searchModel.User
            });
        }

        public ViewResult UploadForm()
        {
            var model = new Track();

            return View(model);
        }

        public ActionResult ListTracks(int startingIndex = 0, TrackSearchModel searchModel = null)
        {
            var tracks = GetTracks(searchModel).Include(t => t.User).OrderByDescending(t => t.UploadDate);

            var viewModel = new ListTracksViewModel()
            {
                Tracks = tracks.Skip(startingIndex).Take(10).ToList(),
                PaginationViewModel = new PaginationViewModel("ListTracks", searchModel, tracks.Count(), 0, 10, startingIndex / 10)
            };

            return View(viewModel);
        }

        public IQueryable<Track> GetTracks(TrackSearchModel searchModel)
        {
            return trackService.GetTracksFromSearchModel(searchModel);
        }

        public ViewResult Details(int id)
        {
            var track = trackService.GetTrackById(id);
            var trackPoints = trackService.GetTrackPointsByTrackId(id);

            TrackDTO trackDTO = AutoMapper.Mapper.Map<Track, TrackDTO>(track);
            List<TrackPointDTO> trackPointsDTO = AutoMapper.Mapper.Map<List<TrackPoint>, List<TrackPointDTO>>(trackPoints);

            var viewModel = new DetailsViewModel()
            {
                Track = trackDTO,
                TrackPoints = trackPointsDTO
            };

            return View(viewModel);
        }

        public ActionResult SimilarTracks(int id)
        {
            var track = trackService.GetTrackById(id);
            var similarTracks = trackService.GetTracksSimilarToTrack(track.Id);

<<<<<<< HEAD
            var viewModel = new SimilarTracksViewModel()
=======
            // Filtering tracks which are close enough to the route
            List<Track> filteredTracks = GeoMath.GetTracksCloseToTrack(similarTrackPoints, trackPoints);

            var viewModel = new DetailsViewModel()
>>>>>>> 857db886105b0c99bcffde990678e1ebee4e99c3
            {
                Track = track,
                SimilarTracks = similarTracks
            };

            return View(viewModel);
        }

        public ViewResult Compare(int trackId1, int trackId2)
        {
<<<<<<< HEAD
            var similarToTrack1 = trackService.GetTrackPointsSimilarToTrack(trackId1, trackId2);

            var trackPoints = trackService.GetTrackPointsByTrackId(trackId1);
            var trackPoints2 = trackService.GetTrackPointsByTrackId(trackId2);
            var track1 = trackService.GetTrackById(trackId1);
            var track2 = trackService.GetTrackById(trackId2);
=======
            // Fetching tracks from database
            List<TrackPoint> trackPoints1 = db.TrackPoints.Where(tp => tp.TrackId == trackId1).OrderBy(tp => tp.Index).ToList();
            List<TrackPoint> trackPoints2 = db.TrackPoints.Where(tp => tp.TrackId == trackId2).OrderBy(tp => tp.Index).ToList();
            List<List<TrackPoint>> segmenty = GeoMath.SplitToSegments(trackPoints2);

            // Filtering track2 points similar to track1
            List<TrackPoint> similarToTrack1 = GeoMath.GetPointsCloseToTrack(trackPoints2, trackPoints1).OrderBy(tp => tp.Index).ToList();
>>>>>>> 857db886105b0c99bcffde990678e1ebee4e99c3

            TrackDTO track1DTO = AutoMapper.Mapper.Map<Track, TrackDTO>(track1);
            TrackDTO track2DTO = AutoMapper.Mapper.Map<Track, TrackDTO>(track2);

            List<TrackPointDTO> trackPointsDTO = AutoMapper.Mapper.Map<List<TrackPoint>, List<TrackPointDTO>>(trackPoints);
            List<TrackPointDTO> trackPoints2DTO = AutoMapper.Mapper.Map<List<TrackPoint>, List<TrackPointDTO>>(trackPoints2);
            // Filtering track1 points similar to track2 filtered points
<<<<<<< HEAD

            var trackSegments1Data = trackService.GetTrackPointsByTrackId(trackId1);
            var trackSegments1DTO = AutoMapper.Mapper.Map<List<TrackPoint>, List<TrackPointDTO>>(trackSegments1Data);
            List<List<TrackPointDTO>> trackSegments1 = new List<List<TrackPointDTO>>
            {
                trackSegments1DTO
            };

            List<TrackSegment> trackSegments2 = GeoMath.SplitToSegments(similarToTrack1);

=======
            List<TrackPoint> similarToFilteredTrack2 = GeoMath.GetPointsCloseToTrack(trackPoints1, similarToTrack1).OrderBy(tp => tp.Index).ToList();

            List<List<TrackPoint>> trackSegments1 = new List<List<TrackPoint>>();
            trackSegments1.Add(db.TrackPoints.Where(tp => tp.TrackId == trackId1).OrderBy(tp => tp.Index).ToList());
            List<List<TrackPoint>> trackSegments2 = GeoMath.SplitToSegments(similarToTrack1);
>>>>>>> 857db886105b0c99bcffde990678e1ebee4e99c3

            var viewModel = new CompareViewModel()
            {
                Track1 = track1DTO,
                Track2 = track2DTO,
                Track2TrackPoints = trackPoints2DTO,
                Segments1 = trackSegments1,
                Segments2 = trackSegments2
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(Track model, HttpPostedFileBase[] postedFiles)
        {
            if (!ModelState.IsValid || postedFiles.Length == 0)
                return View("UploadForm");

            string statusMessage = "";

            GpxParser parser = new GpxParser();

            foreach (var postedFile in postedFiles)
            {
                List<TrackPoint> trackPoints = parser.Parse(postedFile.InputStream);

<<<<<<< HEAD
                if (!trackService.AddTrack(model, trackPoints))
=======
                //string path = Server.MapPath("~/UploadedFiles/");

                List<TrackPoint> trkpts = parser.StreamToTrackPoints(postedFile.InputStream);
                if (!defaultTrackHandler.SetTrackData(model.Name + " (" + postedFile.FileName + ")", model.Description, trkpts))
                    return HttpNotFound();
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
>>>>>>> 857db886105b0c99bcffde990678e1ebee4e99c3
                {
                    statusMessage = "Coś poszło nie tak podczas przetwarzania pliku " + postedFile.FileName;
                    return RedirectToAction("Index", "Home", new { message = statusMessage });
                }
            }

            statusMessage = "Udało się przesłać " + postedFiles.Length + " nowych tras";
            return RedirectToAction("Index", "Home", new { message = statusMessage });
        }

        public ActionResult DuplicateTrack(int trackId, int number)
        {
            trackService.DuplicateTrack(trackId, number);

            return RedirectToAction("TracksTable", "Admin");
        }
    }
}