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
	public class BorderoDepot
    {
		public int BorderoDepotID { get; set; }
       
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime BorderoDepotDate { get; set; }
        [Required]
        //[Index(IsUnique = true)]
        [StringLength(50)]
        public string CodeBorderoDepot { get; set; }
        //cle etrangere vers Assurance
        public int AssureurID { get; set; }
        [ForeignKey("AssureurID")]
        public virtual Assureur Assureur { get; set; }

        public string CompanyID { get; set; }
        public int LieuxdeDepotBorderoID { get; set; }

        public bool ValideBorderoDepot { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ? ValidBorderoDepotDate { get; set; }
        public string HeureGenerateBordero { get; set; }

        public int ? GenerateByID { get; set; }
        [ForeignKey("GenerateByID")]
        public virtual User GenerateBy { get; set; }

        public string HeureValidateBordero { get; set; }
        public int? ValidateByID { get; set; }
        [ForeignKey("ValidateByID")]
        public virtual User ValidateBy { get; set; }
    }
}
