using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Security.Entities
{
    [Serializable]
    public class Adress
    {
        public int AdressID { get; set; }

        //[StringLength(100)]
        //[Index("AdressPhoneNumber", IsUnique = true, IsClustered = false)]
        public string AdressPhoneNumber { get; set; }
				public string AdressCellNumber { get; set; }
				public string AdressFullName { get; set; }
        //[Required]
        //[MinLength(3)]
        //[MaxLength(100)]
        //[Index("AdressEmail", IsUnique = true, IsClustered = false)]
        public string AdressEmail { get; set; }
        public string AdressWebSite { get; set; }
        public string AdressPOBox { get; set; }
        public string AdressFax { get; set; }
        public int QuarterID { get; set; }
        [ForeignKey("QuarterID")]
        public virtual Quarter Quarter { get; set; }
        //public ICollection<Company> Company { get; set; }
        public ICollection<Person> People { get; set; }
    }
}
