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
    public class ExtractSMS
    {
        public int ExtractSMSID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerQuater { get; set; }
        public bool isSmsSent { get; set; }
        [Required]
        [Index("IX_RealPrimaryKey", 6, IsUnique = true, IsClustered = false)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime SaleDeliveryDate { get; set; }
        [Required]
        [Index("IX_RealPrimaryKey", 1, IsUnique = true,IsClustered =false)]
        public int CustomerID { get; set; }
        [Required]
        [Index("IX_RealPrimaryKey", 2, IsUnique = true, IsClustered = false)]
        [StringLength(50)]
        public string AlertDescrip { get; set; }
        [Required]
        [Index("IX_RealPrimaryKey", 3, IsUnique = true, IsClustered = false)]
        [StringLength(50)]
        public string TypeAlert { get; set; }
        [Required]
        [Index("IX_RealPrimaryKey", 4, IsUnique = true, IsClustered = false)]
        [StringLength(50)]
        public string Condition { get; set; }
        [Required]
        [Index("IX_RealPrimaryKey", 5, IsUnique = true, IsClustered = false)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime SendSMSDate { get; set; }

        public bool isDelete { get; set; }
    }
   
}
