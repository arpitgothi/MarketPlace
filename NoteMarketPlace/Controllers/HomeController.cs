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

        public ActionResult SearchNote(int? Type, int? Category, string University, string Country, int? Rating, string Course)
        {
            ViewBag.Message = "Your note page.";

            List<tblSellerNote> tblSellerNotes = context.tblSellerNotes.ToList();
            List<tblCountry> tblCountries = context.tblCountries.ToList();

            var data = from c in tblSellerNotes
                           join t1 in tblCountries on c.Country equals t1.ID
                           where c.Status == 9
                           select new noteData { sellerNote = c, Country = t1 };


            var NoteType = context.tblNoteTypes.ToList();
            SelectList noteTypeList = new SelectList(NoteType, "ID", "Name");
            ViewBag.noteType = noteTypeList;


            var NoteCategory = context.tblNoteCategories.ToList();
            SelectList noteCategoryList = new SelectList(NoteCategory, "ID", "Name");
            ViewBag.noteCategory= noteCategoryList;


            var tblsellernote = context.tblSellerNotes.ToList();

           
            var university = tblsellernote.GroupBy(t => t.UniversityName).Select(g => g.First()).ToList();
            SelectList universityList = new SelectList(university, "universityName", "universityName");
            ViewBag.university = universityList;

            var course = tblsellernote.GroupBy(t => t.Course).Select(g => g.First()).ToList();
            SelectList CourseList = new SelectList(course, "Course", "Course");
            ViewBag.course = CourseList;


            var country = context.tblCountries.ToList();
            SelectList countryList = new SelectList(country, "ID", "Name");
            ViewBag.country = countryList;


            List<SelectListItem> rating = new List<SelectListItem>();
            rating.Add(new SelectListItem() { Value = "1", Text = "1" });
            rating.Add(new SelectListItem() { Value = "2", Text = "2" });
            rating.Add(new SelectListItem() { Value = "3", Text = "3" });
            rating.Add(new SelectListItem() { Value = "4", Text = "4" });
            rating.Add(new SelectListItem() { Value = "5", Text = "5" });

            this.ViewBag.rating = new SelectList(rating, "Value", "Text");

            if (Type != null)
                data = data.Where(m => m.sellerNote.NoteType.Equals(Type));

            if (Category != null)
                data = data.Where(m => m.sellerNote.Category.Equals(Category));

            if (University != null)
                data = data.Where(m => m.sellerNote.UniversityName.Equals(University));

            if (Country != null)
                data = data.Where(m => m.sellerNote.Country.ToString().Equals(Country));

            if (Course != null)
                data = data.Where(m => m.sellerNote.Course.Equals(Course));
            ViewBag.total = data.Count();
            return View(data);
        }
       

        public ActionResult noteDetails()
        {
            ViewBag.Message = "Your faq page.";
            return View();
        }


    }
}