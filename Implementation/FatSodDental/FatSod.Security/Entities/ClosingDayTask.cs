using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Security.Entities
{
    [Serializable]
    public class ClosingDayTask
    {
        public int ClosingDayTaskID { get; set; }
        [Required]
        [StringLength(100)]
        [Index("ClosingDayTaskCode", IsUnique = true, IsClustered = false)]
        public string ClosingDayTaskCode { get; set; }
        public string ClosingDayTaskLabel { get; set; }
        public string ClosingDayTaskDescription { get; set; }
        public virtual ICollection<BranchClosingDayTask> BranchClosingDayTasks { get; set; }
    }
}
