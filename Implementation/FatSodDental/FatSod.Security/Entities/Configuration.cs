using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Security.Entities
{
    [Serializable]
    public class UserConfiguration
    {
        public int UserConfigurationID { get; set; }
        public string DefaultCulture { get; set; }
        public int DefaultBranchID { get; set; }
        [ForeignKey("DefaultBranchID")]
        public virtual Branch DefaultBranch { get; set; }
        public int DefaultDeviseID { get; set; }
        //[ForeignKey("DefaultDeviseID")]
        //public Devise DefaultDevise { get; set; }
        public int? DefaultLocationID { get; set; }
        //[ForeignKey("DefaultLocationID")]
        //public Localization DefaultLocation { get; set; }
        public bool isStockControl { get; set; }
        public bool isDownloadRpt { get; set; }
        public bool isLimitAmountControl { get; set; }
        public bool isTellerControl { get; set; }
    }
}
