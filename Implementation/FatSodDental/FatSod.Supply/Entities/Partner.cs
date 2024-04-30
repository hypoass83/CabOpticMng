using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class Partner
    {
        public int PartnerId { get; set; }
        public string PartnerCode { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Function { get; set; }
        public string Company { get; set; }
        public string ProductsAndServices { get; set; }

    }

}
