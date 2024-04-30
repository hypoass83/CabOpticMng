using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class Operation
    {
        public int OperationID { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]
        public string OperationCode { get; set; }
        public string OperationLabel { get; set; }
        public string OperationDescription { get; set; }
        //cle etrangere vers type operation
        public int OperationTypeID { get; set; }
        [ForeignKey("OperationTypeID")]
        public virtual OperationType OperationType { get; set; }

        //[NotMapped]
        //public string Journal { get; set; }
        
        [NotMapped]
        public string UIoperationTypeCode
        {
            get
            {
                if (OperationType != null)
                {
                    return this.OperationType.operationTypeCode.ToString();
                }
                else return "";
            }
        }

        //public int MacroOperationID { get; set; }
        //[ForeignKey("MacroOperationID")]
        //public virtual MacroOperation MacroOperation { get; set; }

        //[NotMapped]
        //public string UIMacroOperationCode
        //{
        //    get
        //    {
        //        if (MacroOperation != null)
        //        {
        //            return this.MacroOperation.MacroOperationCode.ToString();
        //        }
        //        else return "";
        //    }
        //}

        //public int ReglementTypeID { get; set; }
        //[ForeignKey("ReglementTypeID")]
        //public virtual ReglementType ReglementType { get; set; }

        //[NotMapped]
        //public string UIReglementTypeCode
        //{
        //    get
        //    {
        //        if (ReglementType != null)
        //        {
        //            return this.ReglementType.ReglementTypeCode.ToString();
        //        }
        //        else return "";
        //    }
        //}

        //cle etrangere vers Journal
        public int? JournalID { get; set; }
        [ForeignKey("JournalID")]
        public virtual Journal Journal { get; set; }

        public virtual ICollection<AccountingTask> AccountingTasks { get; set; }
        public virtual ICollection<AccountOperation> AccountOperations { get; set; }
        public virtual ICollection<Piece> Pieces { get; set; }

    }
}
