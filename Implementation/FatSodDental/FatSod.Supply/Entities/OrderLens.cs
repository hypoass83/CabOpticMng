﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class OrderLens : Product
    {

        public EyeSide EyeSide { get; set; }
        public string Addition { get; set; }
        public string Axis { get; set; }
        public string Index { get; set; }

        //Venant de Lens

        //Clés étrangères
        public int LensCategoryID { get; set; }
        [ForeignKey("LensCategoryID")]
        public virtual LensCategory LensCategory { get; set; }
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

        [NotMapped]
        public string LensMaterialCode
        {
            get
            {
                if (LensCategory.LensMaterial != null)
                {
                    return this.LensCategory.LensMaterial.LensMaterialCode.ToString();
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
                if (LensCategory.LensCoating != null)
                {
                    return this.LensCategory.LensCoating.LensCoatingCode.ToString();
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
                if (LensCategory.LensColour != null)
                {
                    return this.LensCategory.LensColour.LensColourCode.ToString();
                }
                else return "";
            }
            set { this.LensColourCode = value; }
        }

        [NotMapped]
        public string LensNumberCylindricalValue { get; set; }
        [NotMapped]
        public string LensNumberSphericalValue { get; set; }

    }
}

