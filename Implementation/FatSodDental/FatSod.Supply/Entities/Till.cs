using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class Till : PaymentMethod
    {
        public int AccountID { get; set; }
        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }
        public int TillMaxMontant { get; set; }
        public string TillComment { get; set; }
        public virtual ICollection<TillSale> TillSales { get; set; }
        /// <summary>
        /// This field allow to show a user that manage this till
        /// </summary>
        /// <returns>
        /// Name of user or employer's name
        /// </returns>
        [NotMapped]
        public string User { get; set; }
    }
    public struct TillSatut
    {
        public double Inputs { get; set; }
        public double Ouputs { get; set; }
        public double OpenningPrice { get; set; }
        public double ClosiningPrice { get; set; }
        public double Ballance { get; set; }
    }
}
