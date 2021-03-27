using NoteMarketPlace.Database;
using NoteMarketPlace.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

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

                if (ModelState.IsValid)
                {

                }


                if (ModelState.IsValid)
                {

                    using (MailMessage m = new MailMessage("aaa@gmail.com", model.Email))
                    {
                        m.Subject = model.Subject;
                        string body = "Hello " + model.Name + ",";
                       

                        m.Body = model.Message;
                        m.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        NetworkCredential NetworkCred = new NetworkCredential("lamdafunction@gmail.com", "*************");
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        smtp.Send(m);
                    }

                }
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
            ViewBag.Message = "Your note page.";

            List<tblSellerNote> tblSellerNotes = context.tblSellerNotes.ToList();
            List<tblCountry> tblCountries = context.tblCountries.ToList();

            var data = from c in tblSellerNotes
                           join t1 in tblCountries on c.Country equals t1.ID
                           where c.Status == 7
                           select new noteData { sellerNote = c, Country = t1 };

            ViewBag.Count = (from c in tblSellerNotes
                             join t1 in tblCountries on c.Country equals t1.ID
                             where c.Status == 7
                             select c).Count();

            return View(data);
        }
        public ActionResult noteDetails()
        {
            ViewBag.Message = "Your faq page.";
            return View();
        }


    }
}