using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Security.Entities
{
    [Serializable]
    public class Sex
    {
        public int SexID { get; set; }
        [Index(IsUnique = true)]
				[StringLength(100)]
        public string SexCode { get; set; }
        public string SexLabel { get; set; }
        public virtual ICollection<Person> People { get; set; }
        [NotMapped]
        public string Civility {
            get {
                return this.SexCode == "M" ? "Civility_M" : "Civility_F";
            }
        }

    }
}
