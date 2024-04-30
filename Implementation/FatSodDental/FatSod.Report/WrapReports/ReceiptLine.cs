using System;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class ReceiptLine
    {
        public int ReceiptLineID { get; set; }
        public double ReceiptLineUnitPrice { get; set; }
        public double ReceiptLineQuantity { get; set; }
        public double ReceiptLineAmount { get; set; }
        public string DetailQty { get; set; }
        public string Designation { get; set; }
        public string ProducType { get; set; }
        public DateTime SaleDate { get; set; }
        public DateTime CommandDate { get; set; }
        public string Reference { get; set; }
        public string NumeroFacture { get; set; }
        public string DeliveryDate { get; set; }
        #region Ajouter pour le bordero de depot de ASCOMA

        public string TreatmentDate { get; set; }
        public string MatriculePatient { get; set; }
        public string TotalMalade { get; set; }
        public string MontantBrut { get; set; } 
        #endregion
    }
}
