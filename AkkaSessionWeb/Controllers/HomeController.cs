using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AkkaSessionWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ChatHub()
        {
            ViewBag.Message = "Chat Hub.";

            return View();
        }

        public ActionResult Session1()
        {
            ViewBag.Message = "Session 1.";

            return View();
        }
        public ActionResult SessionAkka()
        {
            ViewBag.Message = "Session Akka.";

            return View();
        }
    }
}