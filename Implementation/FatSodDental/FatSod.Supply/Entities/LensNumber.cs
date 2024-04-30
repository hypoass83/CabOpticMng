using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class LensNumber
    {        
        public int LensNumberID { get; set; }

        [StringLength(10)]
        [Index("IX_RealPrimaryKey", 1, IsUnique = true)]
        public string LensNumberSphericalValue { get; set; }
        [StringLength(10)]
        [Index("IX_RealPrimaryKey", 2, IsUnique = true)]
        public string LensNumberCylindricalValue { get; set; }
        [StringLength(10)]
        [Index("IX_RealPrimaryKey", 3, IsUnique = true)]
        public string LensNumberAdditionValue { get; set; }
        public string LensNumberDescription { get; set; }
        public virtual ICollection<Lens> Lenses { get; set; }
       [NotMapped]
        protected string LensNumberCylindricalValueCode
        {
            get
            {
                return (this.LensNumberCylindricalValue != null && this.LensNumberCylindricalValue.Length > 0) ? (this.LensNumberCylindricalValue + " " + "CYL") : "";
            }
        }

        [NotMapped]
        protected string LensNumberSphericalValueCode
        {
            get
            {
                return (this.LensNumberSphericalValue != null && this.LensNumberSphericalValue.Length > 0) ? (this.LensNumberSphericalValue + " " + "SPH") : "";
            }
        }

        [NotMapped]
        public string LensNumberFullCode
        {
            get
            {
                string res = "";
                string sphVal = this.LensNumberSphericalValue;
                string cylVal = this.LensNumberCylindricalValue;
                string addVal = this.LensNumberAdditionValue;

                if (sphVal != null && sphVal.Length > 0)
                {
                    if (cylVal != null && cylVal.Length > 0)
                    {

                        if (addVal != null && addVal.Length > 0) // (1 1 1)
                        {
                            res = sphVal + " " + cylVal + " add " + addVal;
                            return res;
                        }

                        if (addVal == null || addVal.Length == 0) //(1 1 0)
                        {
                            res = sphVal + " " + cylVal;
                            return res;
                        }

                    }

                    if (cylVal == null || cylVal.Length == 0)
                    {

                        if (addVal != null && addVal.Length > 0) // (1 0 1)
                        {
                            res = sphVal + " add " + addVal;
                            return res;
                        }

                        if (addVal == null || addVal.Length == 0) //(1 0 0)
                        {
                            res = (sphVal /**/+ " " + "SPH"/**/);
                            return res;
                        }
                    }
                }

                if (cylVal != null && cylVal.Length > 0)
                {
 
                    if (sphVal == null || sphVal.Length == 0)
                    {

                        if (addVal != null && addVal.Length > 0) // (0 1 1)
                        {
                            res = "0.00" + " " + cylVal + " add " + addVal;
                            return res;
                        }

                        if (addVal == null || addVal.Length == 0) //(0 1 0)
                        {
                            res = "0.00" + " " + cylVal ;
                            return res;
                        }
                    }
                }

                throw new Exception("You can not have a Lens Number like that! Please see Administrator for more details");
            }
        }

    }
}
