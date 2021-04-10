using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NoteMarketPlace.Models
{
    public class dashboardpublish
    {
        public bool selltype { get; set; }
        public int notePrice { get; set; }

        public string noteTitle { get; set; }

        public string noteCategory { get; set; }

        public string noteStatus { get; set; }

        public DateTime createdDate { get; set; }
    }
}