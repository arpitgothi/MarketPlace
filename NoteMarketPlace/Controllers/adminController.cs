using NoteMarketPlace.Database;
using NoteMarketPlace.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NoteMarketPlace.Controllers
{
    [Authorize(Roles = "Admin, Super Admin")]
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







        public ActionResult manageNoteType(string search)
        {
            
            ViewBag.Count = 1;
            List<tblNoteType> tblNoteTypesList = context.tblNoteTypes.ToList(); //new List<tblNoteCategory>();
            List<tblUser> tblUser = context.tblUsers.ToList(); //new List<tblNoteCategory>();

            var data = (from c in tblNoteTypesList
                            join t1 in tblUser on c.CreatedBy equals t1.ID
                            select new noteData { NoteType = c, User = t1 }).ToList();
            
            if (!String.IsNullOrEmpty(search))
            {
                data = (from c in tblNoteTypesList
                            join t1 in tblUser on c.CreatedBy equals t1.ID
                            where c.Name.ToLower().Contains(search.ToLower())
                            select new noteData { NoteType = c, User = t1 }).ToList();


            }


            return View(data);
        }
        public ActionResult manageNoteCategory(int? page,string search)
        {
            List<tblNoteCategory> tblNoteCategoriesList = context.tblNoteCategories.ToList(); //new List<tblNoteCategory>();
            List<tblUser> tblUser = context.tblUsers.ToList(); //new List<tblNoteCategory>();

            int pageSize = 5;
            if (page != null)
                ViewBag.Count = page * pageSize - pageSize + 1;
            else
                ViewBag.Count = 1;
            var data = (from c in tblNoteCategoriesList
                            join t1 in tblUser on c.CreatedBy equals t1.ID
                            select new noteData{ NoteCategory = c, User = t1 }).ToList().ToPagedList(page ?? 1, pageSize);
            if (!String.IsNullOrEmpty(search))
            {
                data = (from c in tblNoteCategoriesList
                        join t1 in tblUser on c.CreatedBy equals t1.ID
                        where c.Name.ToLower().Contains(search.ToLower())
                        select new noteData { NoteCategory = c, User = t1 }).ToList().ToPagedList(page ?? 1, pageSize);


            }

            return View(data);
        }
        public ActionResult manageCountry()
        {
            List<tblCountry> tblCountriesList = context.tblCountries.ToList(); //new List<tblNoteCategory>();
            List<tblUser> tblUser = context.tblUsers.ToList(); //new List<tblNoteCategory>();
            ViewBag.Count = 1;
            var data = (from c in tblCountriesList
                            join t1 in tblUser on c.CreatedBy equals t1.ID
                            select new noteData { Country = c, User = t1 }).ToList();


            return View(data);
        }
        public ActionResult manageAdminProfile()
        {
            List<tblCountry> tblCountriesList = context.tblCountries.ToList(); //new List<tblNoteCategory>();
            List<tblUser> tblUser = context.tblUsers.ToList(); //new List<tblNoteCategory>();
            ViewBag.count = 1;
            var data = (from c in tblCountriesList
                            join t1 in tblUser on c.CreatedBy equals t1.ID
                            select new noteData { Country = c, User = t1 }).ToList();


            return View(data);
        }
        public ActionResult myProfile()
        {
            return View();
        }
        public ActionResult manageSysConfig()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult manageSysConfig(systemConfig model)
        {
            string commonpath = Server.MapPath("~/App_Data/a");
            if (!Directory.Exists(commonpath))
            {
                Directory.CreateDirectory(commonpath);
            }

            string defaultnote = Path.GetFileName(model.defaultNoteImage.FileName);
            string defaultnoteimage = Path.Combine(commonpath, defaultnote);
            model.defaultNoteImage.SaveAs(defaultnoteimage);
            string defaultprofile = Path.GetFileName(model.defaultProfilePicture.FileName);
            string defaultprofilepicture = Path.Combine(commonpath, defaultprofile);
            model.defaultProfilePicture.SaveAs(defaultprofilepicture);
            base64converter conv = new base64converter();
         
            if (User.Identity.IsAuthenticated)
            {
                
                string[] modelkey = { "supportMail", "supportPhoneNo", "notificationMail", "facebookURL", "twitterURL", "linkedinURL", "defaultNoteImage", "defaultProfilePicture" };
                string[] modelvalue = { model.supportMail, model.supportPhoneNo, model.notificationMail, model.facebookURL, model.twitterURL, model.linkedinURL, "aaa", "asd" };
                
                for(int i=0; i < modelkey.Length; i++)
                {
                    var temp = modelkey[i];
                    var obj = context.tblSystemConfigurations.Where(m => m.Key.Equals(temp)).FirstOrDefault();
                    obj.Values = modelvalue[i];   
                }
                context.SaveChanges();
            }
            return View();
        }


        public ActionResult noteUnderReview()
        {
            int pageSize = 5;
            var SellerList = context.tblUsers.ToList();
            SelectList list = new SelectList(SellerList, "Id", "FirstName");
            ViewBag.SellerList = list;
           
                ViewBag.Count = 1;

            List<tblSellerNote> tblSellerNotesList = context.tblSellerNotes.ToList();
            List<tblUser> tblUserList = context.tblUsers.ToList();
            List<tblNoteCategory> tblNoteCategoriesList = context.tblNoteCategories.ToList();
            List<tblReferenceData> tblReferenceDataList = context.tblReferenceDatas.ToList();

            var data = (from c in tblSellerNotesList
                        join t1 in tblUserList on c.SellerID equals t1.ID
                        join t2 in tblReferenceDataList on c.Status equals t2.ID
                        join t3 in tblNoteCategoriesList on c.Category equals t3.ID
                        where c.Status == 7 || c.Status == 8
                        select new noteData { sellerNote = c, User = t1, referenceData = t2, NoteCategory = t3 }).ToList();
            return View(data);
        }

        
        [HttpPost]
        public ActionResult Approved(int noteId)
        {
            var obj = context.tblSellerNotes.Where(m => m.ID.Equals(noteId)).FirstOrDefault();


            try
            {
                var admin_id = context.tblUsers.Where(m => m.EmailID.Equals(User.Identity.Name)).FirstOrDefault();
                int id = admin_id.ID;
                obj.Status = 9;
                obj.ActionBy = id;
                obj.PublishedDate = DateTime.Now;
                context.SaveChanges();


            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.PropertyName + ": " + x.ErrorMessage));
                throw new DbEntityValidationException(errorMessages);
            }
            return RedirectToAction("noteUnderReview", "Admin");


        }

        [HttpPost]
        public ActionResult Rejected(int noteId, string rejectRemark)
        {
            var obj = context.tblSellerNotes.Where(m => m.ID.Equals(noteId)).FirstOrDefault();


            try
            {
                var admin_id = context.tblUsers.Where(m => m.EmailID.Equals(User.Identity.Name)).FirstOrDefault();
                int id = admin_id.ID;
                obj.Status = 10;
                obj.AdminRemarks = rejectRemark;
                obj.ActionBy = id;
                context.SaveChanges();


            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.PropertyName + ": " + x.ErrorMessage));
                throw new DbEntityValidationException(errorMessages);
            }
            return RedirectToAction("noteUnderReview", "Admin");


        }



        

        [HttpPost]
        public ActionResult InReview(int noteId)
        {
            var obj = context.tblSellerNotes.Where(m => m.ID.Equals(noteId)).FirstOrDefault();


            try
            {
                var admin_id = context.tblUsers.Where(m => m.EmailID.Equals(User.Identity.Name)).FirstOrDefault();
                int id = admin_id.ID;
                obj.Status = 8;
                obj.ActionBy = id;
                context.SaveChanges();


            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.PropertyName + ": " + x.ErrorMessage));
                throw new DbEntityValidationException(errorMessages);
            }
            return RedirectToAction("noteUnderReview", "Admin");


        }
        [HttpPost]
        public ActionResult Unpublish(int noteId, string removeRemark)
        {
            var obj = context.tblSellerNotes.Where(m => m.ID.Equals(noteId)).FirstOrDefault();


            try
            {
                var admin_id = context.tblUsers.Where(m => m.EmailID.Equals(User.Identity.Name)).FirstOrDefault();
                int id = admin_id.ID;
                obj.Status = 11;
                obj.AdminRemarks = removeRemark;
                obj.ActionBy = id;
                context.SaveChanges();


            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.PropertyName + ": " + x.ErrorMessage));
                throw new DbEntityValidationException(errorMessages);
            }
            return RedirectToAction("dashboard", "Admin");


        }
        
        public ActionResult AdminDownload(int id)

        {

            var tblSeller = context.tblSellerNotes.Where(m => m.ID == id).FirstOrDefault();

            var userId = context.tblUsers.Where(m => m.EmailID == User.Identity.Name && m.RoleID != 103).FirstOrDefault();
            if (userId != null)
            {
                string path = (from sa in context.tblSellerNotesAttachements where sa.NoteID == tblSeller.ID select sa.FilePath).First().ToString();


                string filename = (from sa in context.tblSellerNotesAttachements where sa.NoteID == id select sa.FileName).First().ToString();
                filename += ".pdf";
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);

                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);

            }
            return HttpNotFound();
        }




        public ActionResult manageMember(int? page)
        {
            int pageSize = 5;
            if (page != null)
                ViewBag.Count = page * pageSize - pageSize + 1;
            else
                ViewBag.Count = 1;

            var SellerList = context.tblUsers.ToList();
            SelectList list = new SelectList(SellerList, "Id", "FirstName");
            ViewBag.SellerList = list;


            List<tblSellerNote> tblSellerNotesList = context.tblSellerNotes.ToList();
            List<tblUser> tblUserList = context.tblUsers.ToList();
            List<tblNoteCategory> tblNoteCategoriesList = context.tblNoteCategories.ToList();
            List<tblReferenceData> tblReferenceDataList = context.tblReferenceDatas.ToList();

            var multiple = (from c in tblSellerNotesList
                            join t1 in tblUserList on c.SellerID equals t1.ID
                            join t3 in tblNoteCategoriesList on c.Category equals t3.ID
                            where c.Status == 7 || c.Status == 8
                            select new noteData { sellerNote = c, User = t1, NoteCategory = t3 }).ToList();

            return View(multiple);

        }


        public ActionResult dashboard(int? page)
        {
            int pageSize = 5;
            if (page != null)
                ViewBag.pcount = page * pageSize - pageSize + 1;
            else
                ViewBag.pcount = 1;

            var current_date = DateTime.Now.Date.AddDays(-7);
            ViewBag.notesForReview = context.tblSellerNotes.Where(m => m.Status == 7 || m.Status == 8).Count();
            ViewBag.downloadNotes7db = context.tblDownloads.Where(m => m.IsAttachmentDownloaded == true && m.AttachmentDownloadedDate >= current_date).Count();
            ViewBag.newUser7db = context.tblUsers.Where(m => m.CreatedDate >= current_date).Count();

            List<tblSellerNote> tblSellerNotesList = context.tblSellerNotes.ToList();
            List<tblUser> tblUserList = context.tblUsers.ToList();
            List<tblNoteCategory> tblNoteCategoriesList = context.tblNoteCategories.ToList();
            List<tblReferenceData> tblReferenceDataList = context.tblReferenceDatas.ToList();

            var data = (from c in tblSellerNotesList
                        join t1 in tblUserList on c.SellerID equals t1.ID
                        join t2 in tblReferenceDataList on c.Status equals t2.ID
                        join t3 in tblNoteCategoriesList on c.Category equals t3.ID
                        where c.Status == 9
                        
                        select new noteData { sellerNote = c, User = t1, referenceData = t2, NoteCategory = t3 }).ToList().ToPagedList(page ?? 1, pageSize);
            
            return View(data);
        }
        public ActionResult downloadedNotes()
        {
            List<tblUser> tblUsersList = context.tblUsers.ToList();
            List<tblDownload> tblDownloadList = context.tblDownloads.ToList();
            List<tblUserProfile> tblUserProfilesList = context.tblUserProfiles.ToList();

            int userid = (from user in context.tblUsers where user.EmailID == User.Identity.Name select user.ID).FirstOrDefault();

            var data = from idl in tblDownloadList
                       join t1 in tblUsersList on idl.Downloader equals t1.ID
                       join t2 in tblUserProfilesList on idl.Downloader equals t2.UserID
                       where idl.IsSellerHasAllowedDownload == true
                       select new noteData { download = idl, User = t1, userProfile = t2 };

            return View(data);
        }
        public ActionResult rejectedNotes()
        {
            List<tblUser> tblUsersList = context.tblUsers.ToList();
            List<tblSellerNote> tblSellerNotes = context.tblSellerNotes.ToList();
            List<tblNoteCategory> tblNoteCategories = context.tblNoteCategories.ToList();

            int userId = (from user in context.tblUsers where user.EmailID == User.Identity.Name select user.ID).FirstOrDefault();

            ViewBag.Count = 1;
            var data = (from i in tblSellerNotes
                         join t1 in tblUsersList on i.SellerID equals t1.ID
                        join t2 in tblNoteCategories on i.Category equals t2.ID
                        join t3 in tblUsersList on i.ActionBy equals t3.ID
                        where i.Status == 10

                        select new noteData { sellerNote = i, actionBy=t3 , User = t1, NoteCategory = t2 }).ToList();

            return View(data);
        }


        [Authorize(Roles = "Super Admin")]
        public ActionResult addAdmin()
        {
            NotesMarketPlaceEntities entities = new NotesMarketPlaceEntities();
            var CountryCode = entities.tblCountries.ToList();
            SelectList list = new SelectList(CountryCode, "CountryCode", "CountryCode");
            ViewBag.CountryCode = list;
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Super Admin")]
        public ActionResult addAdmin(userData model)
        {
            NotesMarketPlaceEntities entities = new NotesMarketPlaceEntities();
            var CountryCode = entities.tblCountries.ToList();
            SelectList list = new SelectList(CountryCode, "CountryCode", "CountryCode");
            ViewBag.CountryCode = list;

            string name = User.Identity.Name;
            int u = (from user in context.tblUsers where user.EmailID == name select user.ID).Single();


            if (User.Identity.IsAuthenticated)
            {

                tblUser obj = new tblUser();
                obj.FirstName = model.FirstName;
                obj.LastName = model.LastName;
                obj.EmailID = model.EmailID;
                obj.Password = "Admin@123";
                obj.CreatedDate = DateTime.Now;
                obj.CreatedBy = u;
                obj.IsActive = true;
                obj.IsEmailVerified = true;
                obj.RoleID = 102;

                context.tblUsers.Add(obj);
                context.SaveChanges();


                int id = (from record in context.tblUsers orderby record.ID descending select record.ID).First();

                tblUserProfile userobj = new tblUserProfile();
                userobj.UserID = id;
                userobj.PhoneNumber_CountryCode = model.CountryCode;
                userobj.PhoneNumber = model.PhnNo;
                userobj.AddressLine1 = "addressline1";
                userobj.AddressLine2 = "addressline2";
                userobj.City = "city";
                userobj.State = "State";
                userobj.ZipCode = "362001";
                userobj.Country = "India";
                context.tblUserProfiles.Add(userobj);
                context.SaveChanges();
                ModelState.Clear();
                return RedirectToAction("ManageAdmin", "Admin");


            }
            return View();


        }

    }
}