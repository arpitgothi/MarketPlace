using NoteMarketPlace.Database;
using NoteMarketPlace.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NoteMarketPlace.Controllers
{
    public class HomeController : Controller
    {
        NotesMarketPlaceEntities context = new NotesMarketPlaceEntities();

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

       
        public ActionResult SaveRecord(contact model)
        {
            /* if(ModelState.IsValid)
             {
                 contex.tblContactUs.Add(model);
                 await contex.SaveChangesAsync();
                 return RedirectToAction("Index", "Home");
             }*/
            try
            {

                NotesMarketPlaceEntities context = new NotesMarketPlaceEntities();

                tblContactUs obj = new tblContactUs();
                obj.Name = model.Name;
                obj.EmailID = model.Email;
                obj.Subject = model.Subject;
                obj.Message = model.Message;
                context.tblContactUs.Add(obj);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("Index", "Home");
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