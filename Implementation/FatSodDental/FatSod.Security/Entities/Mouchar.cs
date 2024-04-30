using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Security.Entities
{
    [Serializable]
    public class Mouchar
    {
        public int MoucharID { get; set; }
        public DateTime MoucharDate { get; set; }
        [StringLength(6)]
        public string SneackHour { get; set; }
        public int MoucharUserID { get; set; }
        public string MoucharAction { get; set; }
        public string MoucharDescription { get; set; }
        public string MoucharOperationType{ get; set; }
        public string MoucharProcedureName { get; set; }
        public string MoucharHost { get; set; }
        public string MoucharHostAdress { get; set; }
        public int MoucharBranchID { get; set; }
        public DateTime MoucharBusinessDate { get; set; }
        public Mouchar()
        {
            this.MoucharDate = DateTime.Now;
            this.SneackHour = DateTime.Now.Hour + "h:" + DateTime.Now.Minute;
            this.MoucharHost = Dns.GetHostName();
            this.MoucharHostAdress = Dns.GetHostEntry(this.MoucharHost).AddressList.First().ToString();
        }

    }
    
}
