using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class CumulSaleAndBillLine : Line
    {
        public int CumulSaleAndBillID { get; set; }
        [ForeignKey("CumulSaleAndBillID")]
        public virtual CumulSaleAndBill CumulSaleAndBill { get; set; }
        public string SpecialOrderLineCode { get; set; } //ce code va permettre de connaitre l'unicite d'un verre
        /// <summary>
        /// info pr le frame
        /// </summary>
        public string marque { get; set; }
        public string reference { get; set; }
        /// <summary>
        /// Est ce que le verre a ete commander?
        /// Il faut mettre ce champ ici parce qu'on peut decider de recommander un cote de l'oeil en cas 
        /// de damage par exemple
        /// </summary>
        public bool IsOrdered { get; set; } 

        //info pr le verre

        public string Axis { get; set; }
        public string Addition { get; set; }
        public string Index { get; set; }
        public string LensNumberCylindricalValue { get; set; }
        public string LensNumberSphericalValue { get; set; }

        public int ProductCategoryID { get; set; }

        public double PurchaseLineUnitPrice { get; set; }

        /// <summary>
        /// D'ou viens le produit dont on souhaite faire la sortie en stock
        /// </summary>
        public StockType StockType { get; set; }
        /// <summary>
        /// C'est le fournisseur du verre. 
        /// Ca peut etre JIREH | DBOY | ou un fournisseur externe
        /// </summary>
        public string Supplier { get; set; }
        /// <summary>
        /// C'est le fabriquant du verre
        /// </summary>
        public string Manufacturer { get; set; }
        
        //Caractéristique non persistantes d'un verre de commande; utilisées sur la vue OrderLensOrder
        [NotMapped]
        public EyeSide EyeSide { get; set; }
        
        [NotMapped]
        public string LensCategoryCode { get; set; }
        
    }

    public enum StockType
    {
        NONE, // Rien
        STOCK, // Verre de stock
        STOCK_ORDER, // Verre de stock mais a commander chez Dboy | JIREH ou autre
        RX_ORDER // Verre de Commande
    }

}
