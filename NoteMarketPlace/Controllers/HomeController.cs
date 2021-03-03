using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NoteMarketPlace.Controllers
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
            ModelState.Clear();
            return View();
        }
        public ActionResult faq()
        {
            ViewBag.Message = "Your faq page.";
            return View();
        }

        public ActionResult SearchNote()
        {
            ViewBag.Message = "Your Search Note page.";
            ModelState.Clear();
            return View();
        }
        public ActionResult noteDetails()
        {
            ViewBag.Message = "Your faq page.";
            return View();
        }


    }
}