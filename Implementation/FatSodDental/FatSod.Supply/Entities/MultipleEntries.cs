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
    public class MultipleEntries
		{
		public int BranchID { get; set; }
		public int DeviseID { get; set; }
		public int OperationID { get; set; }
        public int AccountID { get; set; }
        public string AccountSens { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateOperation { get; set; }
		public string Description { get; set; }
		public string Reference { get; set; }
		public string CodeTransaction { get; set; }
		public double Amount { get; set; }
		}
	}
