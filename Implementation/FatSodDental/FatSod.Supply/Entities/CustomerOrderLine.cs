using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class CustomerOrderLine : Line
    {
        public int CustomerOrderID { get; set; }
        [ForeignKey("CustomerOrderID")]
        public virtual CustomerOrder CustomerOrder { get; set; }
        /// <summary>
        /// Pour une commande spéciale, une ligne represente une paire de produit avec un côté gauche et un côté droit de l'oeil
        /// Nous enregistrerons deux lignes dans customerOrderLine.
        /// Ce champ permet de dire que cette ligne represente l'autre côté de la paire.
        /// il sera donc affecter aux deux CustomerOrderLine après la soumission du formulaire
        /// </summary>
        public string SpecialOrderLineCode { get; set; }
        //public double VerreAssuranceCote { get; set; }
        //public double MontureAssuranceCote { get; set; }
        /// <summary>
        /// info pr le frame
        /// </summary>
        public string marque { get; set; }
        public string reference { get; set; }
        public string FrameCategory { get; set; }


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
