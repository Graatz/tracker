using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tracker.Models;
using Tracker.Services;
using Tracker.Strava;

namespace Tracker.Controllers
{
    public class StravaTrackController : Controller
    {
        private IStravaTrackService trackService;

        public StravaTrackController(IStravaTrackService trackService)
        {
            this.trackService = trackService;
        }

        // GET: StravaTrack
        public ActionResult Index()
        {
            return View();
        }

        public ViewResult Authorize(string code)
        {
            if (Session["access_token"] == null)
            {
                if (code == null) return View();
                Session["access_token"] = trackService.Authorize(code);
            }

            var viewModel = new ImportConfirmationViewModel()
            {
                DetailedActivities = trackService.ImportActivities()
            };

            return View("ImportConfirmation", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStravaTracksToDb(ImportConfirmationViewModel model)
        {
            if (model.DetailedActivities.Count <= 0)
                return View("Index", "Home");

            string statusMessage = "";

            foreach (var detailedActivity in model.DetailedActivities)
            {
                if (!detailedActivity.Import)
                    continue;

                if (!trackService.AddTrack(detailedActivity))
                {
                    statusMessage = "Coś poszło nie tak podczas importowania trasy " + detailedActivity.Name + " z aplikacji Strava";
                    return RedirectToAction("Index", "Home", new { message = statusMessage });
                }

            }

            statusMessage = "Udało się zaimportować " + model.DetailedActivities.Where(a => a.Import == true).Count() + " tras z aplikacji Strava";
            return RedirectToAction("Index", "Home", new { message = statusMessage });
        }
    }
}