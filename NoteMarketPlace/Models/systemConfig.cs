using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NoteMarketPlace.Models
{
    public class systemConfig
    {
        [Required]
        public string supportMail{ get; set; }
        [Required]
        public string supportPhoneNo { get; set; }
        [Required]
        public string notificationMail { get; set; }
        [Required]

        public string facebookURL{ get; set; }
        [Required]

        public string twitterURL { get; set; }
        [Required]
        public string linkedinURL { get; set; }
        public HttpPostedFileBase defaultNoteImage { get; set; }
        public HttpPostedFileBase defaultProfilePicture { get; set; }
    }
}