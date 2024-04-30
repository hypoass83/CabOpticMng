using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Security.Entities
{
    [Serializable]
    public class Sneak
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int SneakID { get; set; }
        public DateTime SneakDate { get; set; }
        public string SneakDatabaseTables { get; set; }
        public string SneakDescription { get; set; }
        public string SneakOperationType { get; set; }
        public string SneakOldValue { get; set; }
        public string SneakNewValue { get; set; }
        public string SneakUserNames { get; set; }
        public string SneakHost { get; set; }
        public string SneakHostAdress { get; set; }
        public string SneakBranchNames { get; set; }

        public Sneak() { }

        /// <summary>
        /// Build a Sneak from a SneakDay 
        /// </summary>
        /// <param name="sneakDay"></param>
        public Sneak(SneakDay sneakDay)
        {
            this.SneakID = sneakDay.SneakDayID;
            this.SneakDate = sneakDay.SneakDayDate;
            this.SneakBranchNames = sneakDay.SneakDayBranchNames;
            this.SneakDatabaseTables = sneakDay.SneakDayDatabaseTables;
            this.SneakDescription = sneakDay.SneakDayDescription;
            this.SneakHost = sneakDay.SneakDayHost;
            this.SneakHostAdress = sneakDay.SneakDayHostAdress;
            this.SneakNewValue = sneakDay.SneakDayNewValue;
            this.SneakOldValue = sneakDay.SneakDayOldValue;
            this.SneakOperationType = sneakDay.SneakDayOperationType;
            this.SneakUserNames = sneakDay.SneakDayUserNames;

        }

    }
}
