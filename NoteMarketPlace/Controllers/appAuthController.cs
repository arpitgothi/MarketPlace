using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NoteMarketPlace.Controllers
{
    public class appAuthController : Controller
    {
        // GET: AppAuth
        public ActionResult login()
        {
            return View();
        }

        public ActionResult register()
        {
            return View();
        }

        public ActionResult forgotPassword()
        {
            return View();
        }

    }
    
}