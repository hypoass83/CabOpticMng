using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FatSod.Security.Entities;


namespace FatSod.Supply.Entities
{
    [Serializable]
    public class CompteurBorderoDepot
    {
        public int CompteurBorderoDepotID { get; set; }
        [MaxLength(16)]
        public string CompteurBorderoDepotCode { get; set; }
        public int Counter { get; set; }
        public int YearOperation { get; set; }
        public string CompanyID { get; set; }
        //cle etrangere vers Assurance
        public int  LieuxdeDepotBorderoID { get; set; }
        //[ForeignKey("LieuxdeDepotBorderoID")]
        //public virtual LieuxdeDepotBordero LieuxdeDepotBordero { get; set; }
        //cle etrangere vers Assurance
        public int AssureurID { get; set; }
        [ForeignKey("AssureurID")]
        public virtual Assureur Assureur { get; set; }
    }
}
