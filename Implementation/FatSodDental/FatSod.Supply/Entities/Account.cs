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
    public class Account
    {
        public int AccountID { get; set; }

        [Required]
        [Index(IsUnique = true, IsClustered = false)]
        public int AccountNumber { get; set; }        
        public string AccountLabel { get; set; }
        public bool isManualPosting { get; set; }

        public int CollectifAccountID { get; set; }
        [ForeignKey("CollectifAccountID")]
        public virtual CollectifAccount CollectifAccount { get; set; }

        [NotMapped]
        public string UICollectifAccountNumber
        { 
            get 
            {
                if (CollectifAccount != null) 
                { 
                    return this.CollectifAccount.CollectifAccountNumber.ToString();
                }
                else return "";  
            }
            set { this.UICollectifAccountNumber = value; } 
        }

        public virtual ICollection<AccountingTask> AccountingTasks { get; set; }
        public virtual ICollection<AccountOperation> AccountOperations { get; set; }
        [NotMapped]
        public string CollectifAccountNumber { get; set; }
    }
}
