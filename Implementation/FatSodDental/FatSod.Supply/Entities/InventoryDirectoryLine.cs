﻿using FatSod.Security.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    /// <summary>
    /// cette table contient les infomations sur les modifications de stock.
    /// cette table sera peuplée chaque fois que : 
    /// 1 - la quantité ou le prix de vente unitaire 
    /// </summary>

    public class InventoryDirectoryLine
    {
        public int InventoryDirectoryLineID { get; set; }
        public string inventoryReason { get; set; }
        //nous gardons ici les quantités en stock avant et après l'inventaire 
        public double OldStockQuantity { get; set; }
        public double? NewStockQuantity { get; set; }

        ////nous gardons ici les prix de vente unitaire du produit en stock avant et après l'inventaire         
        //public double  OldStockUnitPrice { get; set; }
        //public double NEwStockUnitPrice { get; set; }
        public double OldSafetyStockQuantity { get; set; }
        public double? NewSafetyStockQuantity { get; set; }
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
        public int LocalizationID { get; set; }
        [ForeignKey("LocalizationID")]
        public virtual Localization Localization { get; set; } 
        public int? AutorizedByID { get; set; }
        [ForeignKey("AutorizedByID")]
        public virtual User AutorizedBy { get; set; }
        public int? CountByID { get; set; }
        [ForeignKey("CountByID")]
        public virtual User CountBy { get; set; }
        public int? RegisteredByID { get; set; }
        [ForeignKey("RegisteredByID")]
        public virtual User RegisteredBy { get; set; }
        public int InventoryDirectoryID { get; set; }
        [ForeignKey("InventoryDirectoryID")]
        public virtual InventoryDirectory InventoryDirectory { get; set; }       
        public double AveragePurchasePrice { get; set; }

        public string NumeroSerie { get; set; }
        public string Marque { get; set; }
        

        [NotMapped]
        public int StockDifference { get; set; }
       [NotMapped]
        public int StockSecurityDifference { get; set; }
       [NotMapped]
       public int TMPID { get; set; }
       [NotMapped]
       public string ProductLabel { get; set; }
       [NotMapped]
       public string LocalizationLabel { get; set; }
       public override bool Equals(object obj)
       {
           if (obj == null) { return false; }
           if (!(obj is InventoryDirectoryLine)) { return false; }
           InventoryDirectoryLine l = (InventoryDirectoryLine)obj;
           return this.ProductID == l.ProductID &&
                  this.LocalizationID == l.LocalizationID &&
                  this.TMPID == l.TMPID
               ;
       }
    }
}
