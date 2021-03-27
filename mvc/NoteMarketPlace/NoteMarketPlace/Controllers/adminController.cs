using NoteMarketPlace.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NoteMarketPlace.Controllers
{
    public class adminController : Controller
    {
        private NotesMarketPlaceEntities context = new NotesMarketPlaceEntities();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }


        //ADD NOTE TYPE SECTION
        public ActionResult addNoteType()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addNoteType(tblNoteType model)
        {
            if (User.Identity.IsAuthenticated)
            {
                var connectionDB = new NotesMarketPlaceEntities();

                string name = User.Identity.Name;
                int cuser = (from user in context.tblUsers where user.EmailID == name select user.ID).Single();
                bool isvalid = context.tblNoteTypes.Any(m => m.Name == model.Name);

                if (!isvalid)
                {
                    tblNoteType obj = new tblNoteType();

                    obj.Name = model.Name;
                    obj.Description = model.Description;
                    obj.CreatedDate = DateTime.Now;
                    obj.CreatedBy = cuser;
                    obj.IsActive = true;

                    if (ModelState.IsValid)
                    {
                        context.tblNoteTypes.Add(obj);
                        try
                        {
                            context.SaveChanges();
                            ModelState.Clear();
                            return View();
                        }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                        ve.PropertyName, ve.ErrorMessage);
                                }
                            }
                        }
                    }
                }
                else
                {
                    ViewBag.Message = "Note Type already exists! Add Another";
                }
            }
            return View();
        }





        public ActionResult addNoteCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addNoteCategory(tblNoteCategory model)
        {
            if (User.Identity.IsAuthenticated)
            {
                string name = User.Identity.Name;
                int cuser = (from user in context.tblUsers where user.EmailID == name select user.ID).Single();
                bool isvalid = context.tblNoteCategories.Any(m => m.Name == model.Name);

                if (!isvalid)
                {
                    tblNoteCategory obj = new tblNoteCategory();
                    obj.Name = model.Name;
                    obj.Description = model.Description;
                    obj.CreatedDate = DateTime.Now;
                    obj.CreatedBy = cuser;
                    obj.IsActive = true;

                    if (ModelState.IsValid)
                    {
                        context.tblNoteCategories.Add(obj);
                        try
                        {
                            context.SaveChanges();
                            ModelState.Clear();

                            return View();

                        }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                        ve.PropertyName, ve.ErrorMessage);
                                }
                            }
                        }

                    }
                }
                else
                {
                    ViewBag.Message = "Note Category already exists";
                }
            }
            return View();
        }




        public ActionResult addCountry()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addCountry(tblCountry model)
        {
            if (User.Identity.IsAuthenticated)
            {
                string name = User.Identity.Name;
                int cuser = (from user in context.tblUsers where user.EmailID == name select user.ID).Single();
                bool isvalid = context.tblCountries.Any(m => m.CountryCode == model.CountryCode);

                if (!isvalid)
                {
                    tblCountry obj = new tblCountry();
                    obj.CountryCode = model.CountryCode;
                    obj.Name = model.Name;
                    obj.CreatedDate = DateTime.Now;
                    obj.CreatedBy = cuser;
                    obj.IsActive = true;
                    
                    if (ModelState.IsValid)
                    {
                        context.tblCountries.Add(obj);
                        try
                        {
                            context.SaveChanges();
                            ModelState.Clear();

                            return View();
                        }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                        ve.PropertyName, ve.ErrorMessage);
                                }
                            }
                        }
                    }
                }
                else
                {
                    ViewBag.Message = "Country already exists in list";
                }
            }
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