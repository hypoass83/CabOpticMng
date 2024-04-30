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
	[NotMapped]
	public class EcritureGLHist
		{
		public int BranchID { get; set; }
        public int DeviseID { get; set; }
		public int OperationID { get; set; }
		public int AccountIDTierCusto { get; set; }
		public int AccountIDTierProduct { get; set; }
		public int AccountIDTresor { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateOperation { get; set; }
		public string Description { get; set; }
		public string Reference { get; set; }
		public string CodeTransaction { get; set; }
		public double MontantPrincDB { get; set; }
        public double MontantPrincCR { get; set; }
        public double Discount { get; set; }
        public double Transport { get; set; }
        public double TVAAmount { get; set; }
       
		public int idSalePurchage { get; set; }

        public double MontantClientDeposit { get; set; } //le montant que le client viens de deposer
        public double MontantTotalClientAdvance { get; set; } //la sommes des avances du clients

    }
	}
