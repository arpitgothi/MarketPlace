using NoteMarketPlace.Database;
using NoteMarketPlace.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NoteMarketPlace.Controllers
{
    [Authorize]

    public class userController : Controller
    {
        // GET: user
        private NotesMarketPlaceEntities context = new NotesMarketPlaceEntities();
        public ActionResult dashboard()
        {
            return View("", "_Layout");
        }


        //ADD NOTE
        public ActionResult addNote()
        {
            NotesMarketPlaceEntities entities = new NotesMarketPlaceEntities();


            var NoteCategory = entities.tblNoteCategories.ToList();
            SelectList list = new SelectList(NoteCategory, "ID", "Name");
            ViewBag.NoteCategory = list;


            var NoteType = entities.tblNoteTypes.ToList();
            SelectList typelist = new SelectList(NoteType, "ID", "Name");
            ViewBag.NoteType = typelist;


            var CountryName = entities.tblCountries.ToList();
            SelectList CountryList = new SelectList(CountryName, "ID", "Name");
            ViewBag.Country = CountryList;



            return View("", "_Layout");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addNote(tblSellerNote model)
        {
            NotesMarketPlaceEntities entities = new NotesMarketPlaceEntities();


            var NoteCategory = entities.tblNoteCategories.ToList();
            SelectList list = new SelectList(NoteCategory, "ID", "Name");
            ViewBag.NoteCategory = list;


            var NoteType = entities.tblNoteTypes.ToList();
            SelectList typelist = new SelectList(NoteType, "ID", "Name");
            ViewBag.NoteType = typelist;


            var CountryName = entities.tblCountries.ToList();
            SelectList CountryList = new SelectList(CountryName, "ID", "Name");
            ViewBag.Country = CountryList;



            string name = User.Identity.Name;
            int uid = (from user in context.tblUsers where user.EmailID == name select user.ID).Single();
            string book_title = model.Title;
            string fullpath_note = null;
            string fullpath_pic = null;
            string fullpath_preview = null;

            tblSellerNote obj = new tblSellerNote();


            int previousID = (from row in context.tblSellerNotes orderby row.ID descending select row.ID).FirstOrDefault();
            previousID += 1;


            string commonpath = Server.MapPath("~/App_Data/");
            if (model.uploadnote != null)
            {
                string notename = Path.GetFileName(model.uploadnote.FileName);
                fullpath_note = Path.Combine(commonpath, notename);
                model.uploadnote.SaveAs(fullpath_note);
            }
            else
            {
                ViewBag.Message = "Please Upload You Note!!!";
                return View();
            }

            if (model.displaypic != null)
            {
                string picname = Path.GetFileName(model.displaypic.FileName);
                fullpath_pic = Path.Combine(commonpath, picname);
                model.displaypic.SaveAs(fullpath_pic);
                obj.DisplayPicture = fullpath_pic;
            }
            if (model.notepreview != null)
            {
                string previewname = Path.GetFileName(model.notepreview.FileName);
                fullpath_preview = Path.Combine(commonpath, previewname);
                model.notepreview.SaveAs(fullpath_preview);
                obj.NotesPreview = fullpath_preview;
            }

            try
            {

                obj.SellerID = uid;
                obj.Title = model.Title;
                obj.Category = model.Category;
                obj.NoteType = model.NoteType;
                obj.NumberofPages = model.NumberofPages;
                obj.Description = model.Description;
                obj.UniversityName = model.UniversityName;
                obj.Country = model.Country;
                obj.Course = model.Course;
                obj.CourseCode = model.CourseCode;
                obj.Professor = model.Professor;
                obj.Status = 7;
                obj.IsPaid = model.IsPaid;
                if (obj.IsPaid)
                {
                    obj.SellingPrice = model.SellingPrice;
                }
                else
                {
                    obj.SellingPrice = 0;
                }
                obj.IsActive = true;

                context.tblSellerNotes.Add(obj);
                context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                throw dbEx;
            }


            int book_id = (from record in context.tblSellerNotes where record.SellerID == uid && record.Title == book_title select record.ID).First();

            try
            {
                tblSellerNotesAttachement S_attachment = new tblSellerNotesAttachement();
                S_attachment.NoteID = book_id;
                S_attachment.FilePath = fullpath_note;
                S_attachment.FileName = book_title;
                S_attachment.CreatedBy = uid;
                S_attachment.CreatedDate = DateTime.Now;
                S_attachment.IsActive = true;
                context.tblSellerNotesAttachements.Add(S_attachment);
                context.SaveChanges();

            }
            catch
            {

            }
            return View("", "_Layout");
        }


        [AllowAnonymous]
        public ActionResult noteDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //tblSellerNote tblSeller = dbobj.tblSellerNotes.Find(id).;
            var userid = context.tblUsers.Where(m => m.EmailID == User.Identity.Name && m.RoleID != 103).FirstOrDefault();
            if (userid != null)
                goto eligible;
            var tblSeller = context.tblSellerNotes.Where(m => m.ID == id && m.Status == 9).FirstOrDefault();
            if (tblSeller == null)
                return HttpNotFound();
            eligible:
            List<tblSellerNote> tblSellerNotes = context.tblSellerNotes.ToList();
            List<tblCountry> tblCountries = context.tblCountries.ToList();
            List<tblNoteCategory> tblNoteCategories = context.tblNoteCategories.ToList();

            var data = from ids in tblSellerNotes
                           join t1 in tblCountries on ids.Country equals t1.ID
                           join t2 in tblNoteCategories on ids.Category equals t2.ID
                           where ids.ID == id
                           select new noteData { sellerNote = ids, Country = t1, NoteCategory = t2 };

            return View(data);
        }


        public ActionResult FreeDownLoad(int? id)
        {


            var user_email = context.tblUsers.Where(m => m.EmailID.Equals(User.Identity.Name)).FirstOrDefault();

            var tblSeller = context.tblSellerNotes.Where(m => m.ID == id).FirstOrDefault();
            var userid = user_email.ID;

            /* if (id == null)
             {
                 return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
             }
             else*/
            if (!tblSeller.IsPaid)
            {
                if (tblSeller == null || tblSeller.Status != 9)
                    return HttpNotFound();

                else if (tblSeller != null && tblSeller.Status == 9)
                {

                    string path = (from sa in context.tblSellerNotesAttachements where sa.NoteID == tblSeller.ID select sa.FilePath).First().ToString();
                    string category = (from c in context.tblNoteCategories where c.ID == tblSeller.Category select c.Name).First().ToString();
                    tblDownload obj = new tblDownload();
                    obj.NoteID = tblSeller.ID;
                    obj.Seller = tblSeller.SellerID;
                    obj.Downloader = userid;
                    obj.IsSellerHasAllowedDownload = true;
                    obj.AttachmentPath = path;
                    obj.IsAttachmentDownloaded = true;
                    obj.IsPaid = false;
                    obj.PurchasedPrice = tblSeller.SellingPrice;
                    obj.NoteTitle = tblSeller.Title;
                    obj.NoteCategory = category;
                    obj.CreatedDate = DateTime.Now;
                    context.tblDownloads.Add(obj);
                    context.SaveChanges();

                    string filename = (from sa in context.tblSellerNotesAttachements where sa.NoteID == id select sa.FileName).First().ToString();
                    filename += ".pdf";
                    byte[] fileBytes = System.IO.File.ReadAllBytes(path);

                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);

                }
            }
            return HttpNotFound();

        }


        public ActionResult AskforDownload(int id)
        {
            var usermail = context.tblUsers.Where(m => m.EmailID.Equals(User.Identity.Name)).FirstOrDefault();

            var tblSeller = context.tblSellerNotes.Where(m => m.ID == id).FirstOrDefault();
            var userid = usermail.ID;
            

            if (tblSeller == null || tblSeller.Status != 9)
                return HttpNotFound();

            else if (tblSeller != null && tblSeller.Status == 9)
            {

                
                string path = (from sa in context.tblSellerNotesAttachements where sa.NoteID == tblSeller.ID select sa.FilePath).First().ToString();
                string category = (from c in context.tblNoteCategories where c.ID == tblSeller.Category select c.Name).First().ToString();
                string seller_name = (from c in context.tblUsers where c.ID == tblSeller.SellerID select c.FirstName).First().ToString();
                string seller_lname = (from c in context.tblUsers where c.ID == tblSeller.SellerID select c.LastName).First().ToString();
                seller_name += " " + seller_lname;
                tblDownload obj = new tblDownload();
                obj.NoteID = tblSeller.ID;
                obj.Seller = tblSeller.SellerID;
                obj.Downloader = userid;
                obj.IsSellerHasAllowedDownload = false;
                obj.AttachmentPath = path;
                obj.IsAttachmentDownloaded = false;
                obj.IsPaid = true;
                obj.PurchasedPrice = tblSeller.SellingPrice;
                obj.NoteTitle = tblSeller.Title;
                obj.NoteCategory = category;
                obj.CreatedDate = DateTime.Now;

                context.tblDownloads.Add(obj);
                context.SaveChanges();
                ViewBag.Msg = "Request Added";

                return Json(new { success = true, responseText = seller_name }, JsonRequestBehavior.AllowGet);
               
            }

            return Json(new { success = false, responseText = "Not Completed." }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult BuyerRequest()
        {
            List<tblUser> tblUsersList = context.tblUsers.ToList();
            List<tblDownload> tblDownloadList = context.tblDownloads.ToList();
            List<tblUserProfile> tblUserProfilesList = context.tblUserProfiles.ToList();

            int userid = (from user in context.tblUsers where user.EmailID == User.Identity.Name select user.ID).FirstOrDefault();
            var data = from i in tblDownloadList
                           join t1 in tblUsersList on i.Downloader equals t1.ID
                           join t2 in tblUserProfilesList on i.Downloader equals t2.UserID
                           where i.Seller == userid && i.IsSellerHasAllowedDownload == false
                           select new noteData { download = i, User = t1, userProfile = t2 };

            return View(data);
        }


        public ActionResult BuyerAllowed(int id)
        {
            NotesMarketPlaceEntities ent = new NotesMarketPlaceEntities();
            var obj = ent.tblDownloads.Where(m => m.ID.Equals(id)).FirstOrDefault();

            if (obj != null)
            {
                obj.IsSellerHasAllowedDownload = true;
                ent.SaveChanges();
            }
            return RedirectToAction("BuyerRequest", "User");
        }



        public ActionResult myDownloads()
        {
            List<tblUser> tblUsersList = context.tblUsers.ToList();
            List<tblDownload> tblDownloadList = context.tblDownloads.ToList();
            List<tblUserProfile> tblUserProfilesList = context.tblUserProfiles.ToList();

            int userid = (from user in context.tblUsers where user.EmailID == User.Identity.Name select user.ID).FirstOrDefault();

            var data = from idl in tblDownloadList
                           join t1 in tblUsersList on idl.Downloader equals t1.ID
                           join t2 in tblUserProfilesList on idl.Downloader equals t2.UserID
                           where idl.Downloader == userid && idl.IsSellerHasAllowedDownload == true
                           select new noteData { download = idl, User = t1, userProfile = t2 };

            return View(data);

        }


        public ActionResult soldNotes()
        {
            
            ViewBag.Count = 1;



            List<tblUser> tblUsersList = context.tblUsers.ToList();
            List<tblDownload> tblDownloadList = context.tblDownloads.ToList();
            List<tblUserProfile> tblUserProfilesList = context.tblUserProfiles.ToList();

            int user_id = (from user in context.tblUsers where user.EmailID == User.Identity.Name select user.ID).FirstOrDefault();

            var data = (from i in tblDownloadList
                            join t1 in tblUsersList on i.Downloader equals t1.ID
                            join t2 in tblUserProfilesList on i.Downloader equals t2.UserID
                            where i.Seller == user_id && i.IsSellerHasAllowedDownload == true


                            select new noteData { download = i, User = t1, userProfile = t2 }).ToList();

            return View(data);

        }

        public ActionResult rejectedNotes()
        {

            List<tblUser> tblUsersList = context.tblUsers.ToList();
            List<tblSellerNote> tblSellerNotes = context.tblSellerNotes.ToList();
            List<tblNoteCategory> tblNoteCategories = context.tblNoteCategories.ToList();

            int user_id = (from user in context.tblUsers where user.EmailID == User.Identity.Name select user.ID).FirstOrDefault();
            
            ViewBag.Count = 1;
            var multiple = (from i in tblSellerNotes
                            join t1 in tblUsersList on i.SellerID equals t1.ID
                            join t2 in tblNoteCategories on i.Category equals t2.ID
                            where i.SellerID == user_id && i.Status == 10


                            select new noteData { sellerNote = i, User = t1, NoteCategory = t2 }).ToList();

            return View(multiple);
        }


        //PROFILE
        public ActionResult profile()
        {
            return View("", "_Layout");
        }
    }
}