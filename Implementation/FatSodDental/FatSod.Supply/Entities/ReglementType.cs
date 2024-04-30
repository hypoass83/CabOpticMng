using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class ReglementType
    {
        public int ReglementTypeID { get; set; }

        [StringLength(30)]
        [Index(IsUnique = true)]
        public string ReglementTypeCode { get; set; }
        public string ReglementTypeLabel { get; set; }
        public string ReglementTypeDescription { get; set; }
        //public virtual ICollection<Operation> Operations { get; set; }
    }
}
