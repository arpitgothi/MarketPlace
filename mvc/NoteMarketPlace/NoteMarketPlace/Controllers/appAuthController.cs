using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using NoteMarketPlace.Database;
using NoteMarketPlace.Models;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Web.Security;
using System.ComponentModel.DataAnnotations;

namespace NoteMarketPlace.Controllers
{
    
    public class appAuthController : Controller
    {
        NotesMarketPlaceEntities dbobj = new NotesMarketPlaceEntities();

        // GET: AppAuth
        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult login(login model)
        {
            var connectionDB = new NotesMarketPlaceEntities();
            bool isvalid = connectionDB.tblUsers.Any(m => m.EmailID == model.Email && m.Password == model.Password);
            if (isvalid)
            {
                var result = connectionDB.tblUsers.Where(m => m.EmailID == model.Email).FirstOrDefault();
                if (result.RoleID == 101 || result.RoleID == 102)
                {
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                    return RedirectToAction("", "Admin");
                }

                else if (result.RoleID == 103)
                {
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                    return RedirectToAction("", "User");
                }
                else
                    ViewBag.NotValidUser = "Something went wrong";
            }
            else
            {
                ViewBag.NotValidUser = "Incorrect Email or Password";
            }
            return View();
        }





        //SignUp Section
        public ActionResult register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult register(tblUser model)
        {

            var connectionDB = new NotesMarketPlaceEntities();
            string email = model.EmailID;
            if (IsValidEmail(email))
            {
                if (model.Password == model.RePassword && model.Password != "")
                {
                    var result = connectionDB.tblUsers.Where(m => m.EmailID == email).FirstOrDefault();
                    if (result == null)
                    {

                        tblUser obj = new tblUser();

                        obj.FirstName = model.FirstName;
                        obj.LastName = model.LastName;
                        obj.EmailID = model.EmailID;
                        obj.Password = model.Password;
                        obj.IsEmailVerified = false;
                        obj.IsActive = true;
                        obj.RePassword = "1223";
                        obj.ModifiedBy = null;
                        obj.ModifiedDate = null;
                        obj.CreatedDate = DateTime.Now;
                        obj.CreatedBy = null;
                        obj.RoleID = 103;
                        if (ModelState.IsValid)
                        {
                            dbobj.tblUsers.Add(obj);
                            try
                            {  
                                dbobj.SaveChanges();
                                ModelState.Clear();
                                FormsAuthentication.SetAuthCookie(model.EmailID, true);
                                return RedirectToAction("Profile", "User");
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
                        return RedirectToAction("faq", "home");
                    }
                    else
                    {
                        ViewBag.UserExist = "This Email is already exists";
                    }
                }
                else
                {
                    ViewBag.NotValidPassword = "Password and Re-enter password must be same";
                }
            }
            else
            {
                ViewBag.NotValidEmail = "Email is not valid";
            }
            return View("register");
        }
        /*public static bool IsValidPassword(string pswd, string repswd)
        {
            if (pswd == repswd && pswd != "")
            {
                return true;
            }
            return false;
        }*/
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }



        public ActionResult forgotPassword()
        {
            return View();
        }

    }
    
}