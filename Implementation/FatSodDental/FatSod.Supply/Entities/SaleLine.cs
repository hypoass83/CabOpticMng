using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class SaleLine : Line
    {
        public int SaleID { get; set; }
        [ForeignKey("SaleID")]
        public virtual Sale Sale { get; set; }
        public string SpecialOrderLineCode { get; set; } //ce code va permettre de connaitre l'unicite d'un verre
        /// <summary>
        /// info pr le frame
        /// </summary>
        public string marque { get; set; }
        public string reference { get; set; }


        //Caractéristique non persistantes d'un verre de commande; utilisées sur la vue OrderLensOrder

        public string Addition { get; set; }
        public string Axis { get; set; }
        public string LensNumberCylindricalValue { get; set; }
        public string LensNumberSphericalValue { get; set; }

        
        [NotMapped]
        public string ProductCategoryID { get; set; }
        [NotMapped]
        public EyeSide EyeSide { get; set; }
        
        
        [NotMapped]
        public string Index { get; set; }
        

        [NotMapped]
        public int PurchaseLineUnitPrice { get; set; }
        [NotMapped]
        public string LensCategoryCode { get; set; }
        
    }
}
