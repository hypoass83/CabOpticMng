using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FatSod.Security.Entities;


namespace FatSod.Supply.Entities
{
    [Serializable]
    public class TransactNumber
    {
        public int TransactNumberID { get; set; }
        [Index(IsUnique = true)]
        [MaxLength(16)]
        public string TransactNumberCode { get; set; }
        public int MaxCounter { get; set; }
        public int Counter { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateOperation { get; set; }
        //cle etrangere vers Agence
        public int? BranchID { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }
    }
}
