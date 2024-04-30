using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Security.Entities
{
    [Serializable]
    public class SneakDay
    {
        public int SneakDayID { get; set; }
        public DateTime SneakDayDate { get; set; }
        public string SneakDayDatabaseTables { get; set; }
        public string SneakDayDescription { get; set; }
        public string SneakDayOperationType{ get; set; }
        public string SneakDayOldValue { get; set; }
        public string SneakDayNewValue { get; set; }
        public string SneakDayUserNames { get; set; }
        public string SneakDayHost { get; set; }
        public string SneakDayHostAdress { get; set; }
        public string SneakDayBranchNames { get; set; }

        public SneakDay()
        {
            this.SneakDayDate = DateTime.Now;
            this.SneakDayHost = Dns.GetHostName();
            this.SneakDayHostAdress = Dns.GetHostEntry(this.SneakDayHost).AddressList.First().ToString();
        }
    }
}
