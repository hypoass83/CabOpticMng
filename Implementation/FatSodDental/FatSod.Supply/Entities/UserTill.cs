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
    public class UserTill
    {
        public int UserTillID { get; set; }
        //Date d'assignation de la caisse à cet utilisateur
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime UserTillDateAssignment { get; set; }
        //C'est le caissier principal
        public bool HasAccess { get; set; }
        public int TillID { get; set; }
        [ForeignKey("TillID")]
        public virtual Till Till { get; set; }
        //L'e,ployé qui est caissier
        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual User USer { get; set; }
        /// <summary>
        /// Date à laquelle l'utilisateur n'est plus caissier
        /// </summary>
        /// 
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? UserTillDisAssignDate { get; set; }
    }
}
