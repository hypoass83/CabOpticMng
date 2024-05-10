using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FatSod.Supply.Entities
{
    /// <summary>
    /// <!--special lens model-->
    /// </summary>
    public class SpecialLensModel
    {
        public int LineID { get; set; }
        public int TMPID { get; set; }
        public int LocalizationID { get; set; }
        public double LineQuantity { get; set; }//C'est la quantité de paire que le client veut
        public double LensPrice { get; set; }//C'est le prix unitaire de la paire
        public string LensDamageComment { get; set; }
        public double LineUnitPrice { get; set; } 
        public string LensCategoryCode { get; set; }
        public string SpecialOrderLineCode { get; set; }
        public string SupplyingName { get; set; }
        public int ProductCategoryID { get; set; }

        //public EyeSide EyeSide { get; set; }
        //Informations pour le côté gauche de l'oeil
        public string LEAddition { get; set; }
        public string LEAxis { get; set; }
        public string LEIndex { get; set; }
        public string LECylinder { get; set; }
        public string LESphere { get; set; }
        public string LEProductCode { get; set; }
        public int LELineID { get; set; }
        public double LEQuantity { get; set; }
        public double LEPrice { get; set; }
        public bool isLCommandGlass { get; set; }

        //Informations pour le côté droit de l'oeil
        public string REAddition { get; set; }
        public string REAxis { get; set; }
        public string REIndex { get; set; }
        public string RECylinder { get; set; }
        public string RESphere { get; set; }
        public string REProductCode { get; set; }
        public int RELineID { get; set; }
        public double REQuantity { get; set; }
        public double REPrice { get; set; }
        public bool isRCommandGlass { get; set; }
        public bool IsRDamaged { get; set; }
        public bool IsLDamaged { get; set; }
        public bool IsFrameDamaged { get; set; }

        public double LensLineQuantity { get; set; }
        

        //information sur le cadre
        public int FrameProductID { get; set; }
        public int FrameQty { get; set; }
        public string marque { get; set; }
        public string reference { get; set; }
        public double FramePrice { get; set; }
        public int FRLineID { get; set; }
        public double FrameLineQuantity { get; set; }
        public string FrameCategory { get; set; }
        public string NumeroSerie { get; set; }
        // pour dire si oui(1) ou non(-1) un cadre vient de la salle VIP
        public int IsVIPRoom { get; set; }

        // Stock Management
        public string RESupplier { get; set; }
        public string LESupplier { get; set; }
        public string REManufacturer { get; set; }
        public string LEManufacturer { get; set; }
        public StockType REStockType { get; set; }
        public StockType LEStockType { get; set; }


        public int PurchaseLineUnitPrice { get; set; }
        public double LineAmount
        {
            get
            {
                return LineQuantity * LineUnitPrice;
            }
        }

        public String ProductLabel { get; set; }

        public String LocalizationLabel { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            if (!(obj is SpecialLensModel)) { return false; }
            SpecialLensModel l = (SpecialLensModel)obj;

            return (l.LEProductCode == this.LEProductCode) && (l.REProductCode == this.REProductCode);
            
        }

        public bool Exist(object obj)
        {
            if (obj == null) { return false; }
            if (!(obj is SpecialLensModel)) { return false; }
            SpecialLensModel l = (SpecialLensModel)obj;
            return this.Equals(l);
        }
    }
}