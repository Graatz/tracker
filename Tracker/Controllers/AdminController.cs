using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tracker.DAL;

namespace Tracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TracksTable()
        {
            var model = unitOfWork.TrackRepository.Get(orderBy: t => t.OrderByDescending(d => d.TrackPoints.Count), includeProperties: "User");

            return View(model);
        }

        public ViewResult UsersTable()
        {
            var model = unitOfWork.UserRepository.Get();

            return View(model);
        }
    }
}