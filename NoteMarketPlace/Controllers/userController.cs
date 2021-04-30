using NoteMarketPlace.Database;
using NoteMarketPlace.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NoteMarketPlace.Controllers
{
    [Authorize]

    public class userController : Controller
    {
        // GET: user
        private NotesMarketPlaceEntities context = new NotesMarketPlaceEntities();

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (requestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                using (var context = new NotesMarketPlaceEntities())
                {
                    // get current user profile image
                    var currentUserImg = (from details in context.tblUserProfiles
                                          join users in context.tblUsers on details.UserID equals users.ID
                                          where users.EmailID == requestContext.HttpContext.User.Identity.Name
                                          select details.ProfilePicture).FirstOrDefault();

                    if (currentUserImg == null)
                    {
                        // get deafult image
                        var defaultImg = context.tblSystemConfigurations.FirstOrDefault(model => model.Key == "defaultProfilePicture");
                        ViewBag.UserProfile = defaultImg.Values;
                    }
                    else
                    {
                        ViewBag.UserProfile = currentUserImg;
                    }
                }
            }
        }
        public userController()
        {

            ViewBag.daefaultNoteImg = context.tblSystemConfigurations.FirstOrDefault(model => model.Key == "defaultNoteImage").Values;
            ViewBag.facebook = context.tblSystemConfigurations.FirstOrDefault(model => model.Key == "facebookURL").Values;
            ViewBag.twitter = context.tblSystemConfigurations.FirstOrDefault(model => model.Key == "twitterURL").Values;
            ViewBag.linkedin = context.tblSystemConfigurations.FirstOrDefault(model => model.Key == "linkedinURL").Values;
        }
        public ActionResult dashboard(int? page)
        {

            var download = context.tblDownloads;
            var sellernote = context.tblSellerNotes;
            var users = context.tblUsers.Where(m => m.EmailID == User.Identity.Name).FirstOrDefault();


            ViewBag.NoOfNotesold = download.Where(m => m.IsSellerHasAllowedDownload == true && m.Seller == users.ID).Count();
            ViewBag.NoOfDownloads = download.Where(m => m.IsSellerHasAllowedDownload == true && m.Downloader == users.ID).Count();
            ViewBag.Earnings = download.Where(m => m.IsSellerHasAllowedDownload == true && m.Seller == users.ID).Sum(m => m.PurchasedPrice);
            ViewBag.RejectedNotes = sellernote.Count(m => m.SellerID == users.ID && m.Status == 10);
            ViewBag.BuyerReqs = download.Where(m => m.IsSellerHasAllowedDownload == false && m.Seller == users.ID).Count();


            var SellerList = context.tblUsers.ToList();
            SelectList list = new SelectList(SellerList, "Id", "FirstName");
            ViewBag.SellerList = list;
            int pageSize = 5;
            if (page != null)
                ViewBag.pcount = page * pageSize - pageSize + 1;
            else
                ViewBag.pcount = 1;
            List<tblSellerNote> tblSellerNotesList = context.tblSellerNotes.ToList();
            List<tblUser> tblUserList = context.tblUsers.ToList();
            List<tblNoteCategory> tblNoteCategoriesList = context.tblNoteCategories.ToList();
            List<tblReferenceData> tblReferenceDataList = context.tblReferenceDatas.ToList();

            var data = (from c in tblSellerNotesList
                                    join t1 in tblUserList on c.SellerID equals t1.ID
                                    join t2 in tblReferenceDataList on c.Status equals t2.ID
                                    join t3 in tblNoteCategoriesList on c.Category equals t3.ID
                                    where c.Status == 7 || c.Status == 8
                                    select new noteData { sellerNote = c, User = t1, referenceData = t2, NoteCategory = t3 }).ToList().ToPagedList(page ?? 1, pageSize);

            var data2 = (from c in tblSellerNotesList
                        join t1 in tblUserList on c.SellerID equals t1.ID
                        join t2 in tblReferenceDataList on c.Status equals t2.ID
                        join t3 in tblNoteCategoriesList on c.Category equals t3.ID
                        where c.Status == 9
                        select new tblSellerNote
                        {
                            Title = c.Title,
                            categoryS = t3.Name,
                            IsPaid = c.IsPaid,
                            SellingPrice = (int)c.SellingPrice,
                            CreatedDate = (DateTime)c.PublishedDate
                        }).ToList();

            ViewBag.publishednotes = data2;

            


            return View(data);
        }
        public PartialViewResult aaa() {
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
            return PartialView(data);
        }

        //ADD NOTE
        public ActionResult addNote()
        {

            var NoteCategory = context.tblNoteCategories.ToList();
            SelectList list = new SelectList(NoteCategory, "ID", "Name");
            ViewBag.NoteCategory = list;


            var NoteType = context.tblNoteTypes.ToList();
            SelectList typelist = new SelectList(NoteType, "ID", "Name");
            ViewBag.NoteType = typelist;


            var CountryName = context.tblCountries.ToList();
            SelectList CountryList = new SelectList(CountryName, "ID", "Name");
            ViewBag.Country = CountryList;


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addNote(tblSellerNote model)
        {
            


            var NoteCategory = context.tblNoteCategories.ToList();
            SelectList list = new SelectList(NoteCategory, "ID", "Name");
            ViewBag.NoteCategory = list;


            var NoteType = context.tblNoteTypes.ToList();
            SelectList typelist = new SelectList(NoteType, "ID", "Name");
            ViewBag.NoteType = typelist;


            var CountryName = context.tblCountries.ToList();
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
                base64converter conv = new base64converter();
                obj.DisplayPicture = conv.converter(fullpath_pic);
                //obj.DisplayPicture = base64converter.converter(fullpath_pic);

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
                obj.CreatedDate = DateTime.Now;
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
                ModelState.Clear();
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
            
            var userid = context.tblUsers.Where(m => m.EmailID == User.Identity.Name && m.RoleID != 103).FirstOrDefault();
            if (userid != null)
                goto eligible;
            var sellerNotes = context.tblSellerNotes.Where(m => m.ID == id && m.Status == 9).FirstOrDefault();
            if (sellerNotes == null)
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


            var userEmail = context.tblUsers.Where(m => m.EmailID.Equals(User.Identity.Name)).FirstOrDefault();
            var sellerNotes = context.tblSellerNotes.Where(m => m.ID == id).FirstOrDefault();
            var userid = userEmail.ID;

            if (!sellerNotes.IsPaid)
            {
                if (sellerNotes == null || sellerNotes.Status != 9)
                    return HttpNotFound();

                else if (sellerNotes != null && sellerNotes.Status == 9)
                {

                    string path = (from sa in context.tblSellerNotesAttachements where sa.NoteID == sellerNotes.ID select sa.FilePath).First().ToString();
                    string category = (from c in context.tblNoteCategories where c.ID == sellerNotes.Category select c.Name).First().ToString();
                    tblDownload obj = new tblDownload();
                    obj.NoteID = sellerNotes.ID;
                    obj.Seller = sellerNotes.SellerID;
                    obj.Downloader = userid;
                    obj.IsSellerHasAllowedDownload = true;
                    obj.AttachmentPath = path;
                    obj.IsAttachmentDownloaded = true;
                    obj.IsPaid = false;
                    obj.PurchasedPrice = sellerNotes.SellingPrice;
                    obj.NoteTitle = sellerNotes.Title;
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
                var seller = context.tblUsers.Where(m => m.ID == tblSeller.SellerID).FirstOrDefault();
                //string sellerName = (from c in context.tblUsers where c.ID == tblSeller.SellerID select c.FirstName).First().ToString();
                //string sellerLname = (from c in context.tblUsers where c.ID == tblSeller.SellerID select c.LastName).First().ToString();
                string sellerName = seller.FirstName + " " + seller.LastName;
                String buyerName = usermail.FirstName + " " + usermail.LastName;
                string buyerMail = seller.EmailID;

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
                ViewBag.Msg = "Request Added Sucessfully";

                string subject = buyerName + " wants to purchase your notes";
                string body = "Hello" + " " + sellerName + ",<br><br>We would like to inform you that, " + buyerName + " wants to purchase your notes." +
                    " Please see Buyer Requests tab and allow download access to Buyer if you have received " +
                    "the payment from him.<br><br>Regards,<br>Notes Marketplace";
                mailAgent mailer = new mailAgent();
                mailer.compose(buyerMail, subject, body);




                return Json(new { success = true, responseText = sellerName }, JsonRequestBehavior.AllowGet);  
            }
            return Json(new { success = false, responseText = "Not Completed." }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult downloadBook(int id)
        {
            NotesMarketPlaceEntities a = new NotesMarketPlaceEntities();
            var user_id = context.tblUsers.Where(m => m.EmailID.Equals(User.Identity.Name)).FirstOrDefault();
            var download = context.tblDownloads.Where(m => m.ID == id && m.Downloader.Equals(user_id.ID)).FirstOrDefault();

            if (download == null || download.IsSellerHasAllowedDownload != true)
                return HttpNotFound();
            else if (download != null)
            {
                string path = download.AttachmentPath;
                string filename = download.NoteTitle;
                filename += ".pdf";

                download.IsAttachmentDownloaded = true;
                download.AttachmentDownloadedDate = DateTime.Now;
                context.SaveChanges();
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);

                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
            }
            return HttpNotFound();
        }

        public ActionResult rejecteddownload(int id)
        {

            var user_id = context.tblUsers.Where(m => m.EmailID.Equals(User.Identity.Name)).FirstOrDefault();
            var download = context.tblDownloads.Where(m => m.NoteID.Equals(id) && m.Seller == user_id.ID).FirstOrDefault();

            var attachments = context.tblSellerNotesAttachements.Where(m => m.NoteID == id).FirstOrDefault();
            var seller = context.tblSellerNotes.Where(m => m.ID == id && m.SellerID == user_id.ID && m.Status == 10).FirstOrDefault();
            if (seller != null || download != null)
            {
                string path = attachments.FilePath;
                string filename = attachments.FileName + ".pdf";
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);

                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
            }
            else {
                return HttpNotFound();
            }
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
            var obj = context.tblDownloads.Where(m => m.ID.Equals(id)).FirstOrDefault();
            var buyer = context.tblUsers.Where(m => m.ID.Equals(obj.Downloader)).FirstOrDefault();
            var seller = context.tblUsers.Where(m => m.ID.Equals(obj.Seller)).FirstOrDefault();
            string buyerName = buyer.FirstName + " " + buyer.LastName;
            string sellerName = seller.FirstName + " " + seller.LastName;
            string buyerMail = buyer.EmailID;

            if (obj != null)
            {
                obj.IsSellerHasAllowedDownload = true;

                context.SaveChanges();

                string subject = sellerName + " Allows you to download a note";
                string body = "Hello " + buyerName +
                   ",<br/><br/>We would like to inform you that," + sellerName + " Allows you to download a note." +
                   "Please login and see My Download tabs to download particular note." +
                   "<br/><br/>Regards,<br/>Notes Marketplace";

                mailAgent mailer = new mailAgent();
                mailer.compose(buyerMail, subject, body);
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
            var data = (from i in tblSellerNotes
                            join t1 in tblUsersList on i.SellerID equals t1.ID
                            join t2 in tblNoteCategories on i.Category equals t2.ID
                            where i.SellerID == user_id && i.Status == 10
                            
                            select new noteData { sellerNote = i, User = t1, NoteCategory = t2 }).ToList();

            return View(data);
        }


        //PROFILE
        public new ActionResult Profile()
        {
            string user_email = User.Identity.Name;
            var user = context.tblUsers.Where(m => m.EmailID == user_email).FirstOrDefault();
            var user_profile = context.tblUserProfiles.Where(m => m.UserID == user.ID).FirstOrDefault();
            if (user_profile != null)
            {
                ViewBag.fname = user.FirstName;
                ViewBag.lnmae = user.LastName;
                ViewBag.Email = user_email;

                //DateTime date = (DateTime)user_profile.DOB;

                //var k = date.Date;

                ViewBag.Dob = "k";
                ViewBag.user_gender = user_profile.Gender.ToString();
                ViewBag.User_code = user_profile.PhoneNumber_CountryCode;
                ViewBag.User_phn = user_profile.PhoneNumber;
                ViewBag.ad1 = user_profile.AddressLine1;
                ViewBag.ad2 = user_profile.AddressLine2;
                ViewBag.city = user_profile.City;
                ViewBag.zip = user_profile.ZipCode;
                ViewBag.user_country = user_profile.Country;
                ViewBag.state = user_profile.State;
                ViewBag.Uni = user_profile.University;
                ViewBag.clg = user_profile.College;
                ViewBag.profileImage = user_profile.ProfilePicture;

                var CountryName = context.tblCountries.ToList();
                List<SelectListItem> CountryList = new SelectList(CountryName, "Name", "Name").ToList();
                CountryList.RemoveAll(i => i.Value == ViewBag.user_country);
                CountryList.Insert(0, (new SelectListItem { Text = ViewBag.user_country, Value = ViewBag.user_country }));
                ViewBag.Country = CountryList;


                var Gender = context.tblReferenceDatas.Where(m => m.RefCategory == "Gender").ToList();
                List<SelectListItem> GenderList = new SelectList(Gender, "ID", "Values").ToList();

                var id = Gender.Where(m => m.ID.Equals(user_profile.Gender)).FirstOrDefault();
                GenderList.RemoveAll(i => i.Value.Equals(ViewBag.user_gender));
                //string mm = ; //dbobj.tblReferenceDatas.Where(m => m.Values == ViewBag.user_gender).FirstOrDefault();  
                GenderList.Insert(0, (new SelectListItem { Text = id.Values, Value = ViewBag.user_gender }));

                ViewBag.Gender = GenderList;

                var Countrycode = context.tblCountries.ToList();
                List<SelectListItem> CcodeList = new SelectList(Countrycode, "CountryCode", "CountryCode").ToList();
                CcodeList.RemoveAll(i => i.Value.Equals(ViewBag.User_code));

                CcodeList.Insert(0, (new SelectListItem { Text = user_profile.PhoneNumber_CountryCode, Value = user_profile.PhoneNumber_CountryCode }));

                ViewBag.Ccode = CcodeList;

            }
            else
            {
                ViewBag.fname = user.FirstName;
                ViewBag.lnmae = user.LastName;
                ViewBag.Email = user_email;
                var CountryName = context.tblCountries.ToList();
                SelectList CountryList = new SelectList(CountryName, "Name", "Name");
                ViewBag.Country = CountryList;


                var Gender = context.tblReferenceDatas.ToList().Where(m => m.RefCategory == "Gender");

                SelectList GenderList = new SelectList(Gender, "Values", "Values");
                ViewBag.Gender = GenderList;

                var Countrycode = context.tblCountries.ToList();
                SelectList CcodeList = new SelectList(Countrycode, "CountryCode", "CountryCode");
                ViewBag.Ccode = CcodeList;
            }
            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public new ActionResult Profile(Profile model)
        {

            var CountryName = context.tblCountries.ToList();
            SelectList CountryList = new SelectList(CountryName, "Name", "Name");
            ViewBag.Country = CountryList;


            var Gender = context.tblReferenceDatas.ToList().Where(m => m.RefCategory == "Gender");
            SelectList GenderList = new SelectList(Gender, "Values", "Values");
            ViewBag.Gender = GenderList;

            var Countrycode = context.tblCountries.ToList();
            SelectList CcodeList = new SelectList(Countrycode, "CountryCode", "CountryCode");
            ViewBag.Ccode = CcodeList;

            var user = context.tblUsers.Where(m => m.EmailID == User.Identity.Name).FirstOrDefault();
            var userProfile = context.tblUserProfiles.Where(m => m.UserID == user.ID).FirstOrDefault();
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            userProfile.DOB = model.DOB;
            userProfile.Gender = model.Gender;
            userProfile.PhoneNumber_CountryCode = model.PhoneNumber_CountryCode;
            userProfile.PhoneNumber = model.PhoneNumber;

            string user_pic = null;
            string defaultpath = Server.MapPath(string.Format("~/Content/Files/{0}", user.ID));
            if (!Directory.Exists(defaultpath))
            {
                Directory.CreateDirectory(defaultpath);
            }

            if (model.ProfilePicture != null)
            {

                string notename = Path.GetFileName(model.ProfilePicture.FileName);
                user_pic = Path.Combine(defaultpath, notename);
                model.ProfilePicture.SaveAs(user_pic);
                base64converter conv = new base64converter();
                userProfile.ProfilePicture = conv.converter(user_pic);
            }



            userProfile.AddressLine1 = model.AddressLine1;
            userProfile.AddressLine2 = model.AddressLine2;
            userProfile.City = model.City;
            userProfile.State = model.State;
            userProfile.ZipCode = model.ZipCode;
            userProfile.Country = model.Country;
            userProfile.College = model.College;
            userProfile.University = model.University;


            context.SaveChanges();

            return RedirectToAction("", "User");
        }


    }
}