using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FatSod.Report.WrapReports
{
    [Serializable]
	public class RptEtatsJournal
		{
		public int RptEtatsJournalID { get; set; }
		[StringLength(50)]
		public string Agence { get; set; }
		[StringLength(100)]
		public string LibAgence { get; set; }
		[StringLength(3)]
		public string Devise { get; set; }
		[StringLength(100)]
		public string LibDevise { get; set; }
		[StringLength(12)]
		public string CompteCle { get; set; }
		[StringLength(100)]
		public string LibelleCpte { get; set; }
		[StringLength(30)]
		public string CodeOperation { get; set; }
		[StringLength(100)]
		public string LibelleOperation { get; set; }
        [StringLength(50)]
        public string Reference { get; set; }
        [StringLength(100)]
        public string Desription { get; set; }
		public DateTime DateOperation { get; set; }
		public double MontantDB { get; set; }
		public double MontantCR { get; set; }
        [StringLength(30)]
        public string CodeTransaction { get; set; }
        [StringLength(30)]
        public string Journal { get; set; }
        
    }
	}
