﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class Lens : Product
    {
        //Clés étrangères
        public int LensCategoryID { get; set; }
        [ForeignKey("LensCategoryID")]
        public virtual LensCategory LensCategory { get; set; }
        [NotMapped] //Provisoirement non persistant
        public string AdditionValue { get; set; }
        [NotMapped]
        public string LensCategoryName
        {
            get
            {
                if (LensCategory != null)
                {
                    return this.LensCategory.CategoryCode;
                }
                else return "";
            }
            set { this.LensCategoryName = value; }
        }

        public int LensNumberID { get; set; }
        [ForeignKey("LensNumberID")]
        public virtual LensNumber LensNumber { get; set; }
        [NotMapped]
        public string LensNumberFullCode
        {
            get
            {
                if (LensNumber != null)
                {
                    return this.LensNumber.LensNumberFullCode.ToString();
                }
                else return "";
            }
            set { this.LensNumberFullCode = value; }
        }
        
    }
}

