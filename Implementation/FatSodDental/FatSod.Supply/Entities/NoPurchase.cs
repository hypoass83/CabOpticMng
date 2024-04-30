using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FatSod.Supply.Entities
{
    [Serializable]
    public class NoPurchase
    {
        public int NoPurchaseId { get; set; }
        
        public int? ConsultDilatationId { get; set; }
        [ForeignKey("ConsultDilatationId")]
        public virtual ConsultDilatation ConsultDilatation { get; set; }
   
        public int? ConsultLensPrescriptionID { get; set; }
        [ForeignKey("ConsultLensPrescriptionID")]
        public virtual ConsultLensPrescription ConsultLensPrescription { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime OperationDate { get; set; }

        public DateTime? CSOperationDate { get; set; } // Date ou le Customer Service a donnee la raison
        
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ConsultationDate { get; set; }

        [Required]
        public string DeliveryDeskReason { get; set; }
        public string CustomerServiceReason { get; set; }

        // Permet de dire si le client est revenu acheter;
        public bool HasBeenPurchased { get; set; }


        [NotMapped]
        public string PrescriptionSummary { get; set; }
        [NotMapped]
        public string DilatationCode { get; set; }
        [NotMapped]
        public string Customer { get; set; }
        [NotMapped]
        public string DisplayDate { get; set; }
        [NotMapped]
        public string DisplayConsultationDate { get; set; }
        [NotMapped]
        public string CustomerValue { get; set; }
        [NotMapped]
        public int Id { get; set; }
        [NotMapped]
        public string OperationType { get; set; } // Dilation | Prescription
        [NotMapped]
        public string Consultant { get; set; }
        [NotMapped]
        public bool IsInsuredCustomer { get; set; }
    }
}
