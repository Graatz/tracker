using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tracker.Models;
using Microsoft.AspNet.Identity;
using Tracker.Helpers;
using System.Diagnostics;
using Microsoft.AspNet.Identity.Owin;
using System.Net;
using System.Data.Entity;

namespace Tracker.Controllers
{
    public class HomeController : Controller
    {
        protected ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Index(string message, int startingIndex = 0)
        {
            var monthSpan = DateTime.Now.AddDays(-30);
            var userId = User.Identity.GetUserId();

            var result = db.Tracks.Where(t => t.UserId.ToString() == userId)
                .Where(t => t.UploadDate >= monthSpan)
                .Include(t => t.User)
                .OrderByDescending(t => t.UploadDate)
                .Skip(startingIndex)
                .Take(10)
                .ToList();

            var viewModel = new IndexViewModel()
            {
                TracksTotal = db.Tracks.Count(),
                UsersTotal = db.Users.Count(),
                TrackPointsTotal = db.TrackPoints.Count(),

                NumberOfTracks = db.Tracks.Where(t => t.UserId.ToString() == userId).Where(t => t.UploadDate >= monthSpan).Count(),
                Tracks = result,
                CurrentPage = (startingIndex / 10) + 1,
                StartingIndex = 0,
                NumberOfTracksPerPage = 10
            };

            if (!string.IsNullOrEmpty(message))
                ViewBag.StatusMessage = message;

            return View(viewModel);
        }
    }
}