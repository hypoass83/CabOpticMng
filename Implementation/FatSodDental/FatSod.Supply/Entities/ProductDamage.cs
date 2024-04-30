using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class ProductDamage
    {
        public int ProductDamageID { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ProductDamageDate { get; set; }
        [Required]
        [StringLength(50)]
        [Index("ProductDamageReference", IsUnique = true, IsClustered = false)]
        public string ProductDamageReference { get; set; }
        public int BranchID { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }

        public int? AutorizedByID { get; set; }
        [ForeignKey("AutorizedByID")]
        public virtual User AutorizedBy { get; set; }
        public int? RegisteredByID { get; set; }
        [ForeignKey("RegisteredByID")]
        //c'est le nom de l'employé de l'agence de départ qui a saisir le transfert
        public virtual User RegisteredBy { get; set; }
        public bool IsLensMountingDamage { get; set; }
        //public int? LensMountingDamageById { get; set; }
        //[ForeignKey("LensMountingDamageById")]
        //public virtual User LensMountingDamageBy { get; set; }
        public string LensMountingDamageBy { get; set; }

        /// <summary>
        /// Pour savoir si la sortie en stock a eu lieu.
        /// Cette variable a pour but de gerer la sortie en stock pour les damages lors du montage
        /// du verre; car sur l'interface Lens Mounting Damage, on enregistre juste sans la sortie en stock
        /// proprement dit
        /// c'est par la suite qu'il y a sortie en stock effective sur l interface Damage Stock Output
        /// </summary>
        public bool IsStockOutPut { get; set; } = true;
        public int? CumulSaleAndBillID { get; set; }
        [ForeignKey("CumulSaleAndBillID")]
        //C'est l'achat qui a ete damage pendant le montage
        public virtual CumulSaleAndBill CumulSaleAndBill { get; set; }

        public virtual ICollection<ProductDamageLine> ProductDamageLines { get; set; }

    }
}
