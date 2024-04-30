using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class AuthoriseSaleLine : Line
    {
        public int AuthoriseSaleID { get; set; }
        [ForeignKey("AuthoriseSaleID")]
        public virtual AuthoriseSale AuthoriseSale { get; set; }
        public string SpecialOrderLineCode { get; set; } //ce code va permettre de connaitre l'unicite d'un verre
        /// <summary>
        /// info pr le frame
        /// </summary>
        public string marque { get; set; }
        public string reference { get; set; }
       
        //info pr le verre

        public string Axis { get; set; }
        public string Addition { get; set; }
        public string Index { get; set; }
        public string LensNumberCylindricalValue { get; set; }
        public string LensNumberSphericalValue { get; set; }

        //Caractéristique non persistantes d'un verre de commande; utilisées sur la vue OrderLensOrder
        [NotMapped]
        public string ProductCategoryID { get; set; }
        [NotMapped]
        public EyeSide EyeSide { get; set; }
        
        

        [NotMapped]
        public int PurchaseLineUnitPrice { get; set; }
        [NotMapped]
        public string LensCategoryCode { get; set; }
        
    }
}
