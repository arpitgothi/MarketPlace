//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NoteMarketPlace.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblDownload
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblDownload()
        {
            this.tblSellerNotesReportedIssues = new HashSet<tblSellerNotesReportedIssue>();
            this.tblSellerNotesReviews = new HashSet<tblSellerNotesReview>();
        }
    
        public int ID { get; set; }
        public int NoteID { get; set; }
        public int Seller { get; set; }
        public int Downloader { get; set; }
        public bool IsSellerHasAllowedDownload { get; set; }
        public string AttachmentPath { get; set; }
        public bool IsAttachmentDownloaded { get; set; }
        public Nullable<System.DateTime> AttachmentDownloadedDate { get; set; }
        public bool IsPaid { get; set; }
        public Nullable<decimal> PurchasedPrice { get; set; }
        public string NoteTitle { get; set; }
        public string NoteCategory { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
    
        public virtual tblSellerNote tblSellerNote { get; set; }
        public virtual tblUser tblUser { get; set; }
        public virtual tblUser tblUser1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSellerNotesReportedIssue> tblSellerNotesReportedIssues { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSellerNotesReview> tblSellerNotesReviews { get; set; }
    }
}
