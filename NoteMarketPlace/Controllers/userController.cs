using NoteMarketPlace.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            int u = (from user in context.tblUsers where user.EmailID == name select user.ID).Single();
            string book_title = model.Title;
            string fullpath_note = null;
            string fullpath_pic = null;
            string fullpath_preview = null;

            tblSellerNote obj = new tblSellerNote();

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

                obj.SellerID = u;
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
                obj.IsActive = true;
                obj.SellingPrice = model.SellingPrice;
                obj.IsPaid = model.IsPaid;

                context.tblSellerNotes.Add(obj);
                context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                throw dbEx;
            }


            int book_id = (from record in context.tblSellerNotes where record.SellerID == u && record.Title == book_title select record.ID).First();

            try
            {
                tblSellerNotesAttachement S_attachment = new tblSellerNotesAttachement();
                S_attachment.NoteID = book_id;
                S_attachment.FilePath = fullpath_note;
                S_attachment.FileName = book_title;
                S_attachment.CreatedBy = u;
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





        //PROFILE
        public ActionResult profile()
        {
            return View("", "_Layout");
        }
    }
}