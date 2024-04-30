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
    public class CollectifAccount
    {
        public int CollectifAccountID { get; set; }

        [Required]
        [Index(IsUnique = true, IsClustered = false)]
        public int CollectifAccountNumber { get; set; }
        public string CollectifAccountLabel { get; set; }
        
        public int AccountingSectionID { get; set; }
        [ForeignKey("AccountingSectionID")]
        public virtual AccountingSection AccountingSection { get; set; }

        //public void Returns<T>(Func<object, object> p)
        //{
        //    throw new NotImplementedException();
        //}

        [NotMapped]
        public string UIAccountingSectionNumber { 
            get 
            {
                if (AccountingSection != null) 
                { 
                    return this.AccountingSection.AccountingSectionNumber.ToString();
                }
                else return "";  
            }
            set { this.UIAccountingSectionNumber = value; } 
        }

        //public void Returns(CollectifAccount retcollAcct)
        //{
        //    return retcollAcct;
        //}

        [NotMapped]
        public string AccountingSectionNumber { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
