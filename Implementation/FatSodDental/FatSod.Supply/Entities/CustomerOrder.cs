using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class CustomerOrder
    {
        public int CustomerOrderID { get; set; }
        public int CompteurFacture { get; set; }
        /// <summary>
        /// Il est important d'indiquer si une vente est urgente
        /// </summary>
        public bool IsUrgent { get; set; }
        //Ce champ represente PostedDate dans la commande spéciale
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime CustomerOrderDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? CustomerDateHours { get; set; }
        
        public double VatRate { get; set; }
        public double RateReduction { get; set; } //sera ossi use coe taux de prise en charge pr assure
        public double RateDiscount { get; set; }
        public string Patient { get; set; }
        
        [Index(IsUnique = true)]
        [StringLength(100)]
        public string CustomerOrderNumber { get; set; }
        public bool IsDelivered { get; set; }
        //customer and insurance info
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string InsurreName { get; set; }

        public string PhoneNumber { get; set; }
        [StringLength(250)]
        public string PoliceAssurance { get; set; }

        [StringLength(250)]
        public string CompanyName { get; set; }

        public int? CustomerID { get; set; }
        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }

        //company d'assurance
        public int? AssureurID { get; set; }
        [ForeignKey("AssureurID")]
        public virtual Assureur Assureur { get; set; }

        //Lieux de depot Bordero
        public int? LieuxdeDepotBorderoID { get; set; }
        [ForeignKey("LieuxdeDepotBorderoID")]
        public virtual LieuxdeDepotBordero LieuxdeDepotBordero { get; set; }

        //Company du client
        public int? InsuredCompanyID { get; set; }
        [ForeignKey("InsuredCompanyID")]
        public virtual InsuredCompany InsuredCompany { get; set; }

        public int DeviseID { get; set; }
        [ForeignKey("DeviseID")]
        public virtual Devise Devise { get; set; }
        public int BranchID { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }
        public int OperatorID { get; set; }
        [ForeignKey("OperatorID")]
        public virtual User Operator { get; set; }
        public virtual ICollection<CustomerOrderLine> CustomerOrderLines { get; set; }
        public string Remarque { get; set; }
        public string MedecinTraitant { get; set; }
        /// <summary>
        /// C est la data a laquelle l assure a ete consultee
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? TreatmentDate { get; set; }
        public double Transport { get; set; }
        public double PlafondAssurance { get; set; }

        public double RemiseAssurance { get; set; }

        public string NumeroBonPriseEnCharge { get; set; }
        public double VerreAssurance { get; set; }
        public double MontureAssurance { get; set; }
        /// <summary>
        /// Montant Net de la prestation(Ascoma)
        /// </summary>
        public double Plafond { get; set; }
        /// <summary>
        /// Ticket Moderateur(Ascoma)
        /// </summary>
        public double TotalMalade { get; set; }
        // C'est la valeur(VIP ou ECO) du client; elle ne doit changer que si elle etait differente de VIP; car un client qui a deja ete 
        // VIP le demeure
        public CustomerValue CustomerValue { get; set; }
        // C'est la derniere valeur du client, Etant donnee qu'un client VIP ne peut plus devenir  
        /// <summary>
        /// LastCustomerValue; C'est la derniere valeur(VIP | ECO) du client
        ///  Etant donnee qu'un client VIP ne peut plus devenir ECO et
        ///  qu'on doit savoir si par rapport au dernier achat du client sa valeur a baisse,
        ///  cet champ contient la valeur du client en fonction de son dernier achat
        /// </summary>
        public CustomerValue LastCustomerValue { get; set; }

        [StringLength(100)]
        public string NumeroFacture { get; set; }
        /// <summary>
        /// bill state:
        /// - Proforma
        /// - Validate
        /// </summary>
        public StatutFacture BillState { get; set; }
        /// <summary>
        /// Detail sur les montants de la facture
        /// 0 = No
        /// 1 = Yes
        /// </summary>
        public int DatailBill { get; set; }
        //public double MntReduction { get; set; }
        public double MntValidate { get; set; }

        //Marketer
        public int? GestionnaireID { get; set; }
        [ForeignKey("GestionnaireID")]
        public virtual User Gestionnaire { get; set; }

        //seller
        public int? SellerID { get; set; }
        [ForeignKey("SellerID")]
        public virtual User Seller { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ValidateBillDate { get; set; }

        public int? BorderoDepotID { get; set; }
        [ForeignKey("BorderoDepotID")]
        public virtual BorderoDepot BorderoDepot { get; set; }

        public string DeleteReason { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DeleteBillDate { get; set; }
        public int? DeletedByID { get; set; }
        [ForeignKey("DeletedByID")]
        public virtual User DeletedBy { get; set; }

        public double MntValideBordero { get; set; }
        public bool isMntValideBordero { get; set; }


        [NotMapped]
        public int TotalPriceHT { get; set; }
        [NotMapped]
        public int TotalPriceTTC { get; set; }
        [NotMapped]
        public double ReductionAmount { get; set; }
        [NotMapped]
        public double DiscountAmount { get; set; }
        [NotMapped]
        public double TVAAmount { get; set; }

        [NotMapped]
        public double Remainder { get; set; }
        [NotMapped]
        public double SaleTotalPrice { get; set; }
        [NotMapped]
        public string CustomerFullName { get; set; }
        [NotMapped]
        public double Advanced { get; set; }
        [NotMapped]
        public string AssureurName { get; set; }

        [NotMapped]
        public int PaymentMethodID { get; set; }
        [NotMapped]
        public string PaymentReference { get; set; }

        [NotMapped]
        public int GlobalPersonID { get; set; }
        [NotMapped]
        public int SexID { get; set; }
        [NotMapped]
        public int AdressID { get; set; }
        [NotMapped]
        public string CustomerNumber { get; set; }
        [NotMapped]
        public string CNI { get; set; }
        [NotMapped]
        public string DateOfBirth { get; set; }
        [NotMapped]
        public string PreferredLanguage { get; set; }
        [NotMapped]
        public string CustomerValueUI
        {
            get
            {
                return this.CustomerValue == CustomerValue.VIP ? "VIP" : "ECO";
            }
        }

    }
}
