using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NoteMarketPlace.Controllers
{
    //[Authorize]

    public class userController : Controller
    {
        // GET: user
        public ActionResult dashboard()
        {
            return View("", "_Layout");
        }
        public ActionResult addNote()
        {
            return View("","_Layout");
        }
        public ActionResult profile()
        {
            return View("", "_Layout");
        }
    }
}