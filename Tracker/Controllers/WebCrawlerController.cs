using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tracker.DAL;
using Tracker.Helpers;
using Tracker.Models;
using Tracker.Services;

namespace Tracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class WebCrawlerController : Controller
    {
        protected UnitOfWork unitOfWork = new UnitOfWork();
        // GET: WebCrawler
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OsmWebCrawler()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CrawlPage(int page)
        {
            ICrawlerService osmCrawler = new OpenStreetMapCrawlerService(unitOfWork);
            try
            {
                osmCrawler.DownloadTracksFromPage(page);
            }
            catch (Exception ex)
            {
                return Json("Wystąpił błąd podczas pobierania strony " + page, JsonRequestBehavior.AllowGet);
            }

            return Json("Sukces! Udało się pobrać stronę nr. " + page, JsonRequestBehavior.AllowGet);
        }
    }
}