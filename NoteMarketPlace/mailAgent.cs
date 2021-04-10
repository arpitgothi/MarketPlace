using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Helpers;

namespace NoteMarketPlace
{
    public class mailAgent 
    {
        public void compose(string email,  string subject , string body)
        {
            
            var fromEmail = new MailAddress("***********@gmail.com", "lamda Function");
            var toEmail = new MailAddress(email);
            var fromemailPassword = "*************";
            string[] asto = { "arpitgothi002@gmail.com" };


            

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new NetworkCredential(fromEmail.Address, fromemailPassword)
            };
            using (var message = new MailMessage(fromEmail, toEmail))
            {
                foreach (var rec in asto)
                {
                    message.CC.Add(new MailAddress(rec));
                }
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                smtp.Send(message);
            };
        }
    }
}