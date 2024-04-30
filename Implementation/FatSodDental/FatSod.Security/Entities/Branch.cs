
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Security.Entities
{
    [Serializable]
    public class Branch
    {
        public int BranchID { get; set; }
        [Required]
        [StringLength(100)]
        [Index("BranchCode", IsUnique = true, IsClustered = false)]
        public string BranchCode { get; set; }
       
        [Required]
        [StringLength(10)]
        //[Index("BranchAbbreviation", IsUnique = true, IsClustered = false)]
        public string BranchAbbreviation { get; set; }

        [Required]
        [StringLength(100)]
        [Index("BranchName", IsUnique = true, IsClustered = false)]
        public string BranchName { get; set; }
        public string BranchDescription { get; set; }
        public int AdressID { get; set; }
        [ForeignKey("AdressID")] 
        public virtual Adress Adress { get; set; }
        public int CompanyID { get; set; }
        [ForeignKey("CompanyID")]
        public virtual Company Company { get; set; }
        public virtual ICollection<UserBranch> UserBranches { get; set; }
        public virtual ICollection<BusinessDay> BusinessDays { get; set; }
        public virtual ICollection<BranchClosingDayTask> BranchClosingDayTasks { get; set; }
        
    }
}
