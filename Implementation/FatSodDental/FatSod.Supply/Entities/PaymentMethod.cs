using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public abstract class PaymentMethod
    {
        public int ID { get; set; }
        //[Index(IsUnique = true)]
        [StringLength(100)]
        public string Name { get; set; }
        [Index(IsUnique = true)]
        [StringLength(100)]
        public string Code { get; set; }
        public string Description { get; set; }
        //public int AccountID { get; set; }
        //[ForeignKey("AccountID")]
        //public virtual Account Account { get; set; }
        public int BranchID { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }

        public virtual ICollection<Deposit> Deposits { get; set; }
        public virtual ICollection<Slice> Slices { get; set; }

    }
}