﻿using FatSod.Security.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    /// <summary>
    /// cette table contient les infomations sur les modifications de stock.
    /// cette table sera peuplée chaque fois que : 
    /// 1 - la quantité ou le prix de vente unitaire 
    /// </summary>
 [Serializable]
    public class InventoryHistoric
    {
        public int InventoryHistoricID { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime InventoryDate { get; set; }
        public string inventoryReason { get; set; }
        //nous gardons ici les quantités en stock avant et après l'inventaire 
        public double OldStockQuantity { get; set; }
        public double NewStockQuantity { get; set; }

        //nous gardons ici les prix de vente unitaire du produit en stock avant et après l'inventaire         
        public double  OldStockUnitPrice { get; set; }
        public double NEwStockUnitPrice { get; set; }
        public double OldSafetyStockQuantity { get; set; }
        public double NewSafetyStockQuantity { get; set; }
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
        public int LocalizationID { get; set; }
        [ForeignKey("LocalizationID")]
        public virtual Localization Localization { get; set; } 
        public int AutorizedByID { get; set; }
        [ForeignKey("AutorizedByID")]
        public virtual User AutorizedBy { get; set; }
        public int? CountByID { get; set; }
        [ForeignKey("CountByID")]
        public virtual User CountBy { get; set; }
        public int RegisteredByID { get; set; }
        [ForeignKey("RegisteredByID")]
        public virtual User RegisteredBy { get; set; }
         /// <summary>
         /// Le stockStatus peut etre INPUT ou OUTPUT
         /// </summary>
         /// 
        [Required]
        [StringLength(6)]
        public string StockStatus { get; set; }
        [Required]
        public string Description { get; set; }
        public double Quantity { get; set; }

        public string NumeroSerie { get; set; }
        public string Marque { get; set; }
        
    }
}
