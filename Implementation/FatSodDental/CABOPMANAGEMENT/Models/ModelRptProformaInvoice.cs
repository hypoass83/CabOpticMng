using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CABOPMANAGEMENT.Models
{
    [Serializable]
    public class ModelRptProformaInvoice
    {
        public int ModelRptProformaInvoiceID { get; set; }
        [Required]
        [StringLength(100)]
        public string Reference { get; set; }
        public DateTime ProformaDate { get; set; }
        //Header
        public string Title { get; set; }
        public string TitleFr { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAdress { get; set; }
        public string CompanyTel { get; set; }
        public string CompanyCNI { get; set; }
        
        //End header=============================
        public string MontantLettreFr { get; set; }
        public string MontantLettreEn { get; set; }
        //===== Customer identification
        public string CustomerName { get; set; }
        /// <summary> 
        /// Agence de (Douala ou Yaounde) qui doit apparaitre sur la facture proforma
        /// Pour le moment, ca correspond avec la ville de l agence
        /// </summary>
        public string Agency { get; set; }
        public string InsurreName { get; set; }
        //public string CustomerAdress { get; set; } //sera utiliser pour l'email de l'agence
        //public string CustomerAccount { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerCompany { get; set; }
        public string CustomerDoctor { get; set; }
        public DateTime PrescriptionDate { get; set; }
        //=========
        public double TotalAmount { get; set; }
        public string Operator { get; set; }
        public string DeviseLabel { get; set; }
       
        //public double Balance { get; set; }
        //public double TotalAmountTTC { get; set; }
        //public string Ref { get; set; }

        public double TotalLens { get; set; }
        public double TotalFrame { get; set; }

        public virtual ICollection<RxPrescription> RxPrescription { get; set; }
        //public virtual ICollection<DetailFrame> DetailFrames { get; set; }
        
    }
}
