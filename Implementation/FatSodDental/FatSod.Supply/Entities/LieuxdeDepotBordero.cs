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
    [Serializable]
    public class LieuxdeDepotBordero
    {
        public int LieuxdeDepotBorderoID { get; set; }


        [StringLength(100)]
        [Index("IX_LieuxdeDepotBordero", IsUnique = true, IsClustered = false)]
        public string LieuxdeDepotBorderoName { get; set; }

        public virtual ICollection<CustomerOrder> CustomerOrders { get; set; }

    }
   
}
