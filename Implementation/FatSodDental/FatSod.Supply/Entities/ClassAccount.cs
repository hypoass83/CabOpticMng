using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class ClassAccount
    {
        public int ClassAccountID { get; set; }

        [Index("ClassAccountNumber", IsUnique = true, IsClustered = false)]
        public int ClassAccountNumber { get; set; }
        [Required]
        [StringLength(100)]
        [Index("ClassAccountCode",IsUnique = true, IsClustered = false)]
        public string ClassAccountCode { get; set; }
        public string ClassAccountLabel { get; set; }
        public virtual ICollection<AccountingSection> AccountingSections { get; set; }
    }
}
