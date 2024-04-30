﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class LensCategory : Category
    {
        [StringLength(10)]
        public string BifocalCode { get; set; }
        public Boolean IsProgressive { get; set; } //Code = PRO       
        //Clés étrangères
        public int LensMaterialID { get; set; }
        [ForeignKey("LensMaterialID")]
        public virtual LensMaterial LensMaterial { get; set; }

        public int CollectifAccountID { get; set; }
        [ForeignKey("CollectifAccountID")]
        public virtual CollectifAccount CollectifAccount { get; set; }

        public int LensCoatingID { get; set; }
        [ForeignKey("LensCoatingID")]
        public virtual LensCoating LensCoating { get; set; }
        public int LensColourID { get; set; }
        [ForeignKey("LensColourID")]
        public virtual LensColour LensColour { get; set; }
        public string SupplyingName { get; set; }
        public bool IsSpecialCategory { get; set; }
        [StringLength(10)]
        public string LensIndex { get; set; }
        public int LensDiameter { get; set; }
        /// <summary>
        /// - Progressif
        /// - Single Vision
        /// - Bifocal D-Top
        /// - Bifocal R-Top
        /// </summary>
        public string TypeLens { get; set; }
        //Champs Non Persistants

        [NotMapped]
        public string LensMaterialCode
        {
            get
            {
                if (LensMaterial != null)
                {
                    return this.LensMaterial.LensMaterialCode.ToString();
                }
                else return "";
            }
            set { this.LensMaterialCode = value; } 
        }
        
        [NotMapped]
        public string LensCoatingCode
        {
            get
            {
                if (LensCoating != null)
                {
                    return this.LensCoating.LensCoatingCode.ToString();
                }
                else return "";
            }
            set { this.LensCoatingCode = value; } 
        }
        
        [NotMapped]
        public string LensColourCode
        {
            get
            {
                if (LensColour != null)
                {
                    return this.LensColour.LensColourCode.ToString();
                }
                else return "";
            }
            set { this.LensColourCode = value; } 
        }
        [NotMapped]
        public string CollectifAccountNumber
        {
            get
            {
                if (CollectifAccount != null)
                {
                    return this.CollectifAccount.CollectifAccountNumber.ToString();
                }
                else return "";
            }
            set { this.CollectifAccountNumber = value; }
        }
    }
}

