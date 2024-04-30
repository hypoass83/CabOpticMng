using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Security.Entities
{
    [Serializable]
    /// <summary>
    /// cette table contiendra les opérations à éffectuer avant de fermer une journée
    /// lors de la création d'une agence, cette table sera remplie avec les opérations de fin de journées backup, contrôle de la caisse, contrôle des écritures comptbales
    /// </summary>
    public class BranchClosingDayTask
    {
        public int BranchClosingDayTaskID { get; set; }
        //public DateTime DateOperation { get; set; }
        public bool Statut { get; set; }
        public int ClosingDayTaskID { get; set; }
        [ForeignKey("ClosingDayTaskID")]
        public virtual ClosingDayTask ClosingDayTask { get; set; }
        public int BranchID { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }
       
    }
}
