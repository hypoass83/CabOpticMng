using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Security.Entities
{
    [Serializable]
    public class Company : GlobalPerson
    {
        public int CompanyCapital { get; set; }
        [StringLength(50)]
        [Index("CompanySigle", IsUnique = true, IsClustered = false)]
        public string CompanySigle { get; set; }
        [StringLength(50)]
        [Index("CompanyTradeRegister", IsUnique = true)]
        public string CompanyTradeRegister { get; set; }
        public string ONOCNumber { get; set; }
        public string CompanySlogan { get; set; }
        public bool CompanyIsDeletable { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
        public virtual ICollection<Branch> Branches { get; set; }
    }
}
