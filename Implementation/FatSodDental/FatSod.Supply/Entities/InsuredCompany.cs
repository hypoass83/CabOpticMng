using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class InsuredCompany
    {        
        public int InsuredCompanyID { get; set; }
        [Required]
        [StringLength(100)]
        [Index("XI_InsuredCompany", IsUnique = true, IsClustered = false)]
        public string InsuredCompanyCode { get; set; }
        public string InsuredCompanyLabel { get; set; }
        
        public virtual ICollection<CustomerOrder> CustomerOrders { get; set; }
    }
}
