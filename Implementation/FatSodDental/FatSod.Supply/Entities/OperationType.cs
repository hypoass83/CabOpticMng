using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class OperationType
    {
        public int operationTypeID { get; set; }

        [StringLength(30)]
        [Index(IsUnique = true)]
        public string operationTypeCode { get; set; }
        public string operationTypeLabel { get; set; }
        public string operationTypeDescription { get; set; }
        public virtual ICollection<Operation> Operations { get; set; }

        

    }
}
