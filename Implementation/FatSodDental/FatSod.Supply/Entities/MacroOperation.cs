using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class MacroOperation
    {
        public int MacroOperationID { get; set; }

        [StringLength(30)]
        [Index(IsUnique = true)]
        public string MacroOperationCode { get; set; }
        public string MacroOperationLabel { get; set; }
        public string MacroOperationDescription { get; set; }
        //public virtual ICollection<Operation> Operations { get; set; }
    }
}
