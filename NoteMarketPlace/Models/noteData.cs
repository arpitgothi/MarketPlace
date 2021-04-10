using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NoteMarketPlace.Models
{
    public class noteData
    {
        public Database.tblUser User { get; set; }
        public Database.tblNoteCategory NoteCategory { get; set; }
        public Database.tblNoteType NoteType { get; set; }
        public Database.tblSellerNote sellerNote { get; set; }
        public Database.tblUserProfile userProfile { get; set; }
        public Database.tblCountry Country { get; set; }
        public Database.tblDownload download { get; set; }
        public Database.tblReferenceData referenceData { get; set; }
        public Database.tblUser actionBy { get; set; }
        public int totalDownloads { get; set; }
    }


}