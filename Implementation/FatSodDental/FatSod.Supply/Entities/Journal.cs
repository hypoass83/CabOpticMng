using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class Journal
    {
        public int JournalID { get; set; }

        [StringLength(30)]
        [Index(IsUnique = true)]
        public string JournalCode { get; set; }
        public string JournalLabel { get; set; }
        public string JournalDescription { get; set; }
        public virtual ICollection<Operation> Operations { get; set; }
    }
}
