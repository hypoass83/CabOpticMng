﻿using FatSod.Security.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    /// <summary>
    /// cette table contient les infomations sur les codes bares qui seront generer.
    /// </summary>

    public class BarCodeGenerator
    {
        public int BarCodeGeneratorID { get; set; }
        [StringLength(30)]
        public string CodeBar { get; set; }
        public int CompteurCodeBar { get; set; }
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
        public int GeneratedByID { get; set; }
        [ForeignKey("GeneratedByID")]
        public virtual User GeneratedBy { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime GenerateDate { get; set; }     
        public int QtyGenerate { get; set; }
        [NotMapped]
        public string ProductCode { get; set; }
    }
}
