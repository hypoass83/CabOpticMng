using System;

namespace CABOPMANAGEMENT.Areas.CRM.ViewModel
{
    public class PatientRecordViewModel
    {
        public int Order { get; set; }
        public int OperationId { get; set; }
        public string OperationDate { get; set; }
        // Registration; Parameters; etc.
        public string OperationType { get; set; }
        public string OperationSummary { get; set; }
        public string HasBeenPurchased { get; set; }
        public string Operator { get; set; }
        public string OperationAmount { get; set; }
        public string OperationReference { get; set; }
        // House; Insured; Cash; House-Insured
        public string CustomerType { get; set; }
        public DateTime DateOperationHours { get; set; }

    }
}