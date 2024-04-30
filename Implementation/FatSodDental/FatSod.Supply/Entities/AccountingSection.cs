using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FatSod.Supply.Entities
{
    [Serializable]
    //entite chapitre comptable
    public class AccountingSection
    {
       
        public int AccountingSectionID { get; set; }
        [Index(IsUnique = true, IsClustered = false)]
        public int AccountingSectionNumber { get; set; }
        [Required]
        [StringLength(100)]
        [Index("CategoryCode", IsUnique = true, IsClustered = false)]
        public string AccountingSectionCode { get; set; }
        public string AccountingSectionLabel { get; set; }
        
        //foreign key vers classaccount
        public int ClassAccountID { get; set; }
        [ForeignKey("ClassAccountID")]
        public virtual ClassAccount ClassAccount { get; set; }


        [NotMapped]
        public string UIClassAccountNumber 
        { 
            get
            {
                if (ClassAccount != null) { return this.ClassAccount.ClassAccountNumber.ToString(); }
                else return "";
                
            } 
        }

        public virtual ICollection<CollectifAccount> CollectifAccounts { get; set; }
        public virtual ICollection<AccountingTask> AccountingTasks { get; set; }
    }
}
