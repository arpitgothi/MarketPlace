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
using System.Net.Mail;
using System.Net;

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
                    return RedirectToAction("dashboard", "Admin");
                }

                else if (result.RoleID == 103)
                {
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                    
                    if(dbobj.tblUserProfiles.Where(m=> m.UserID==result.ID).FirstOrDefault() == null)
                    {
                        return RedirectToAction("profile", "user");
                    }
                    return RedirectToAction("dashboard", "User");
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
                    var result = IsEmailExist(email);
                    if (result==false)
                    {

                        tblUser obj = new tblUser();
                        obj.FirstName = model.FirstName;
                        obj.LastName = model.LastName;
                        obj.EmailID = model.EmailID;
                        //obj.Password = crypto.Hash(model.Password);
                        obj.Password = model.Password;
                        obj.IsEmailVerified = false;
                        obj.IsActive = true;
                        obj.RePassword = "1234";
                        obj.ModifiedBy = null;
                        obj.ModifiedDate = null;
                        obj.CreatedDate = DateTime.Now;
                        obj.CreatedBy = null;
                        obj.RoleID = 103;

                        obj.ActivationCode = Guid.NewGuid();
                        if (ModelState.IsValid)
                        {
                            dbobj.tblUsers.Add(obj);
                            
                            try
                            {  
                                dbobj.SaveChanges();
                                ModelState.Clear();
                                sendEmailVerificationLink(model.EmailID,obj.ActivationCode.ToString());
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
                        ViewBag.Exist = "This Email is already exists";
                    }
                }
                else
                {
                    ViewBag.notvalidpass = "Password and Re-enter password must be same";
                }
            }
            else
            {
                ViewBag.notvalidEmail = "Email is not valid";
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
        public static bool IsEmailExist(string email)
       {
            var connectionDB = new NotesMarketPlaceEntities();
            var v = connectionDB.tblUsers.Where(m => m.EmailID == email).FirstOrDefault();
            return v !=null;
       }

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
        //Verify Account  

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (NotesMarketPlaceEntities dc = new NotesMarketPlaceEntities())
            {
                dc.Configuration.ValidateOnSaveEnabled = false; // This line I have added here to avoid 
                                                                // Confirm password does not match issue on save changes
                var v = dc.tblUsers.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    dc.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.Status = Status;
            return View();
        }


        /*public void sendEmailVerificationLink(String email, string activationCode)
        {
            var verifyUrl = "/appAuth/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var fromEmail = new MailAddress("tempmail7895@gmail.com", "lamda Function");
            var toEmail = new MailAddress(email);
            var fromemailPassword = "ASDfghjkL.123@";
            string subject = "Your account is sucesfully created";
            string body = "<br> we are exicited to tell you that your account is sucesfully created " +
                "please click on the below link to verify the account <br> " +
                "<a href='" + link + "'>" + link + "</a>";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new NetworkCredential(fromEmail.Address,fromemailPassword)
            };
            using (var message = new MailMessage(fromEmail, toEmail) { Subject = subject, Body = body, IsBodyHtml = true })
                smtp.Send(message);
        }*/
        public void sendEmailVerificationLink(String email, string activationCode, String emailFor = "VerifyAccount")
        {
            var verifyUrl = "/appAuth/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var fromEmail = new MailAddress("***********@gmail.com", "lamda Function");
            var toEmail = new MailAddress(email);
            var fromemailPassword = "*************";
            string tempPass = activationCode;
            string subject = "";
            string body = "";



            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is sucesfully created";
                body = "<br> we are exicited to tell you that your account is sucesfully created " +
                    "please click on the below link to verify the account <br> " +
                    "<a href='" + link + "'>" + link + "</a>";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "New Password for your account!";
                body = "<br> Hii,<br> We got request for reset Password of your account" +
                    "please use this password temporarily for the account <br> " +
                    "Temp Password :" + tempPass;
            }

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new NetworkCredential(fromEmail.Address, fromemailPassword)
            };
            using (var message = new MailMessage(fromEmail, toEmail) { Subject = subject, Body = body, IsBodyHtml = true })
                smtp.Send(message);
        }



        //FORGOT PASSWORD SECTION
        public ActionResult forgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult forgotPassword(tblUser model)
        {
            string message = "";
            bool Status = false;
            using (NotesMarketPlaceEntities fp = new NotesMarketPlaceEntities())
            {
                var account = fp.tblUsers.Where(a => a.EmailID == model.EmailID).FirstOrDefault();
                if (account != null)
                {
                    Random r = new Random();
                    string resetCode = r.Next(10001,99999).ToString();
                    sendEmailVerificationLink(account.EmailID, resetCode, "ResetPassword");
                    account.Password = resetCode;
                    fp.Configuration.ValidateOnSaveEnabled = false;
                    fp.SaveChanges();
                }
                else
                {
                    message = "Account not found";
                }
            }

            return View();
        }
        public ActionResult logout()
        {
            FormsAuthentication.SignOut();
            return View("login","appAuth");
        }




    }
    
}