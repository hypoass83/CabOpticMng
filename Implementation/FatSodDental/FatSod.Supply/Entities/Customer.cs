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
    public class Customer : Person
    {
       
        /// <summary>
        /// cpte collectif des clients
        /// </summary>
        public int AccountID { get; set; }
        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }

        /// <summary>
        /// C'est la langue par defaut d'une personne.
        /// </summary>
        public string PreferredLanguage { get; set; }

        ///// <summary>
        ///// customer number
        ///// </summary>
        //[Index("CustomerNumber", IsUnique = true, IsClustered = false)]
        //[StringLength(250)]
        //public string CustomerNumber { get; set; }

        ///// <summary>
        ///// Permet de dire si oui ou non un client est un client assurer
        ///// </summary>
        // public bool IsInsuredCustomer { get; set; }

        public bool IsInHouseCustomer { get; set; } //Check in house Customer

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

        public virtual ICollection<CustomerOrder> CustomerOrders { get; set; }

        /// <summary>
        /// PoliceAssurance
        /// </summary>
        //[Index("PoliceAssurance", IsUnique = true, IsClustered = false)]
        [StringLength(250)]
        public string PoliceAssurance { get; set; }

        [StringLength(250)]
        public string CompanyName { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateOfBirth { get; set; }

        public bool IsBillCustomer { get; set; } //true if its an insured customer
        [StringLength(10)]
        public string CustomerNumber { get; set; }

        [StringLength(250)]
        public string Profession { get; set; }

        
        public int? AssureurID { get; set; }
        [ForeignKey("AssureurID")]
        public virtual Assureur Assureur { get; set; }
          
        //gestionnaire de compte
        public int? GestionnaireID { get; set; }
        [ForeignKey("GestionnaireID")]
        public virtual User Gestionnaire { get; set; }
        //limite montant pour non client
        public double LimitAmount { get; set; }
        //date enregistretement

        public DateTime? Dateregister { get; set; }

        [NotMapped]
        public string CustomerFullName
        {
            get
            {
                return String.Concat(this.Name, " ", (this.Description != null) ? this.Description : "");
            }
        }

        [NotMapped]
        public string CustomerValueUI
        {
            get
            {
                return this.CustomerValue == CustomerValue.VIP ? "VIP" : "ECO";
            }
        }
        

        [NotMapped]
        public string AssureurName
        {
            get
            {
                if (Assureur != null)
                    return this.Assureur.Name.ToString();
                else
                    return "";
            }
        }

        [NotMapped]
        public double Debt { get; set; }

        /// <summary>
        /// This a NotMapped attribute is not persiste in database and readonly. 
        /// It return a Account.AccountNumber value of this Person
        /// </summary>
        /// 
        [NotMapped]
        public string AccountNumber
        {
            get
            {
                if (Account != null)
                    return this.Account.AccountNumber.ToString();
                else
                    return "";
            }
        }
        /// <summary>
        /// This a NotMapped attribute is not persiste in database and readonly. 
        /// It return a Account.AccountLabel value of this Person
        /// </summary>
        [NotMapped]
        public string AccountLabel
        {
            get
            {
                if (Account != null)
                    return this.Account.AccountLabel;
                else
                    return "";
            }
        }

        /// <summary>
        /// Mettre a jour la valeur(VIP | ECO) d'un client (apres une vente)
        /// </summary>
        /// <param name="saleAmount">saleAmount; c'est le Montant total de la vente</param>
        /// <param name="lensCategory">lensCategory; c'est la categorie du verre</param>
        /// <param name="isInsuredCustomer">isInsuredCustomer; c'est pour dire c'est un client assurer ou pas</param>
        public static CustomerValue getCustomerValue(double saleAmount, LensCategory lensCategory, bool isInsuredCustomer)
        {
            CustomerValue res = CustomerValue.ECO;

            if (isInsuredCustomer == true && saleAmount >= 150000)
            {
                res = CustomerValue.VIP;
            }

            if (isInsuredCustomer == false)
            {
                if (lensCategory == null)
                {
                    if (saleAmount >= 60000) // achay d'autre chose qui n'est pas les verres(Monture)
                    {
                        res = CustomerValue.VIP;
                    }
                }
                else
                {
                    if (lensCategory.IsProgressive && saleAmount >= 120000)
                    {
                        res = CustomerValue.VIP;
                    }

                    if (!lensCategory.IsProgressive && saleAmount >= 60000)
                    {
                        res = CustomerValue.VIP;
                    }
                }
                
            }

            return res;
        }

    }

    //public enum InHouseCustomer
    //{
    //    Cash,
    //    NonCash,
    //    Both
    //}

    public enum CustomerValue
    {
        // Si les conditions de VIP ne sont pas respectees
        ECO,

        // 1- Si client cash,
            // 1.a- Le verre est un Simple vision et le total de la facture est superieur ou egale a 60 000
            // 1.b- Le verre est progressif et le total de la facture est superieur a 120 000
            // 1.c- C'est juste le frame et le montant est superieur a 60 000
        // 2- Si client assure,
            // 2.a- Le total de la facture est superieur ou egale a 150 000
        VIP
    }
}
