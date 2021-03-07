using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NoteMarketPlace.Controllers
{
    public class adminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult addNoteType()
        {
            return View();
        }
        public ActionResult addNoteCategory()
        {
            return View();
        }
        public ActionResult addCountry()
        {
            return View();
        }
        public ActionResult manageNoteType()
        {
            return View();
        }
        public ActionResult manageNoteCategory()
        {
            return View();
        }
        public ActionResult manageCountry()
        {
            return View();
        }
        public ActionResult manageAdminProfile()
        {
            return View();
        }
        public ActionResult myProfile()
        {
            return View();
        }
        public ActionResult manageSysConfig()
        {
            return View();
        }
        public ActionResult noteUnderReview()
        {
            return View();
        }
        public ActionResult publishedNotes()
        {
            return View();
        }
        public ActionResult downloadedNotes()
        {
            return View();
        }
        public ActionResult rejectedNotes()
        {
            return View();
        }
        public ActionResult spamReport()
        {
            return View();
        }
        public ActionResult manageMember()
        {
            return View();
        }
        public ActionResult addAdmin()
        {
            return View();
        }
    }
}