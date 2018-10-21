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

        public ActionResult Index()
        {
            var monthSpan = DateTime.Now.AddDays(-30);
            var userId = User.Identity.GetUserId();

            var viewModel = new Tracker.Models.IndexViewModel()
            {
                Tracks = db.Tracks.Where(t => t.UserId.ToString() == userId).Where(t => t.UploadDate >= monthSpan).ToList(),
                NumberOfTracks = db.Tracks.Count(),
                NumberOfUsers = db.Users.Count(),
                NumberOfTrackPoints = db.TrackPoints.Count()
            };

            return View(viewModel);
        }
    }
}