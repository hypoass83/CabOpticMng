using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class CumulSaleAndBill
    {
        public int CumulSaleAndBillID { get; set; }
        public double VatRate { get; set; }
        public double RateReduction { get; set; }
        public double RateDiscount { get; set; }
        public double Transport { get; set; }
        /// <summary>
        /// Il est important d'indiquer si une vente est urgente
        /// </summary>
        public bool IsUrgent { get; set; }
        public bool IsOrdered { get; set; } // Est ce que le verre a ete commander
        /// <summary>
        /// C'est l'utilisateur qui a poster la commande
        /// </summary>
        public int? OrderById { get; set; }
        [ForeignKey("OrderById")]
        public virtual User OrderBy { get; set; }
        /// <summary>
        /// C'est l'utilisateur qui a controlle que la commande a ete bien passee
        /// </summary>
        public int? OrderControllerId { get; set; }
        [ForeignKey("OrderControllerId")]
        public virtual User OrderController { get; set; }
        /// <summary>
        /// Les raisons pour lesquelles la vente a ete commander plus d'une fois.
        /// Les raison seront separees par des |
        /// Reason1 | Reason2
        /// </summary>
        public string ReOrderReasons { get; set; }
        /// <summary>
        /// Les dates de recommandes.
        /// Les dates seront separees par des |
        /// date1 | date2
        /// </summary>
        public string ReOrderDates { get; set; }
        /// <summary>
        /// C'est la date a laquelle la commande a ete faite.
        /// Dans le cas des recommandes, ici on aura la derniere date de commande
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? OrderDate { get; set; }
        /// <summary>
        /// C'est le nombre de fois que la vente a ete commandee
        /// </summary>
        public int NumberOfTimesOrdered { get; set; }
        public bool IsReceived { get; set; } // Est ce que le verre de command est receptionner?
        public bool IsMounted { get; set; } // Est ce que le verre est monter?
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime SaleDate { get; set; }
        [Index(IsUnique = true)]
        [StringLength(100)]
        public string SaleReceiptNumber { get; set; }
        public int BranchID { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }
        public string CustomerName { get; set; }
        public int? CustomerID { get; set; }
        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }
        public bool isReturn { get; set; }
        /// <summary>
        /// Pour savoir si la sortie en stock a eu lieu.
        /// </summary>
        public bool IsStockOutPut { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateOperationHours { get; set; }

        // public DateTime StockOutPutOperationHours { get; set; }

        public virtual ICollection<CumulSaleAndBillLine> CumulSaleAndBillLines { get; set; }

        public int? SaleID { get; set; }
        [ForeignKey("SaleID")]
        public virtual Sale Sale { get; set; }

        public int? CustomerOrderID { get; set; }
        [ForeignKey("CustomerOrderID")]
        public virtual CustomerOrder CustomerOrder { get; set; }

        public int? OperatorID { get; set; }
        [ForeignKey("OperatorID")]
        public virtual User Operator { get; set; }

        public OriginSaleOperation OriginSale { get; set; } //bill ou sale
        public string Remarque { get; set; }
        public string MedecinTraitant { get; set; }

        /*************** variable pour la gestion des rendez vs pour les verres de commande *******************/
        public bool IsRendezVous { get; set; } //cette variable permet de determiner si une vente sera considerer comme commande speciale
        public int? AwaitingDay { get; set; } //nombre probable de jour d'attente
        public bool IsProductReveive { get; set; } //determine la reception du produit
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? EffectiveReceiveDate { get; set; } //date effective de reception du produit
        public bool IsCustomerRceive { get; set; } //si reception par le client
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? CustomerDeliverDate { get; set; } //date de reception par le client

        public int? PostSOByID { get; set; }
        [ForeignKey("PostSOByID")]
        public virtual User PostSOBy { get; set; }

        public int? ReceiveSOByID { get; set; }
        [ForeignKey("ReceiveSOByID")]
        public virtual User ReceiveSOBy { get; set; }

        // C'est l'identifiant de celui qui a receptionner le verre de commande
        // Pour le moment, c'est l'utilisateur connectee
        public int? ReceiverID { get; set; }
        [ForeignKey("ReceiverID")]
        public virtual User Receiver { get; set; }

        // parametre pour la livraison du produit
        public bool IsDeliver { get; set; } //si livraison au client
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DeliverDate { get; set; } //date de livraison au client

        public int? DeliverByID { get; set; }
        [ForeignKey("DeliverByID")]
        public virtual User DeliverBy { get; set; }

        public bool IsProductDeliver { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ProductDeliverDate { get; set; } //date de livraison du produit au client
        public string MountingBy { get; set; }
        public string ControlBy { get; set; }
        // Commentaire de celui qui fait le controle
        public string LensMountingComment { get; set; }

        public DateTime? ProductDeliverDateHeure { get; set; }
        public DateTime? SpecialOrderReceptionDateHeure { get; set; } // Date et heure de reception verre de commande
        public DateTime? LensMountingDateHeure { get; set; } // Date et heure a laquelle le verre a ete montee

        public int? DeliverProductByID { get; set; }
        [ForeignKey("DeliverProductByID")]
        public virtual User DeliverProductBy { get; set; }

        [NotMapped]
        public int spray { get; set; }

        [NotMapped]
        public int cases { get; set; }
        
        [NotMapped]
        public int robs { get; set; }
        [NotMapped]
        public string ReOrderReason { get; set; }
        [NotMapped]
        public bool IsREOrdered { get; set; }
        [NotMapped]
        public bool IsLEOrdered { get; set; }
        [NotMapped]
        public bool IsRECommandGlass { get; set; }
        [NotMapped]
        public bool IsLECommandGlass { get; set; }
        [NotMapped]
        public int RELineId { get; set; }
        [NotMapped]
        public int LELineId { get; set; }        
        [NotMapped]
        public string IsUrgentUI
        {
            get
            {
                return this.IsUrgent ? "YES" : "NO";
            }
        }

        public void UpdateReOrderReasons(string newReOrderReason)
        {
            this.ReOrderReasons = this.ReOrderReasons == null ? "" : this.ReOrderReasons;
            this.ReOrderReasons = newReOrderReason != null ? (this.ReOrderReasons + "|" + newReOrderReason) :
                                                               this.ReOrderReasons;
        }

        public List<string> GetReOrderReasons()
        {
            List<string> res = new List<string>();
            if (this.ReOrderReasons != null)
            {
                string[] reasons = this.ReOrderReasons.Split('|');
                foreach(string reason in reasons)
                {
                    string reasonUI = (reason == null || reason == "") ? "RAS" : reason;
                    res.Add(reasonUI);
                }
            }
            
            return res;
        }

        public void UpdateReOrderDates(DateTime newReOrderDate)
        {
            this.ReOrderDates = this.ReOrderDates == null ? "" : this.ReOrderDates;
            this.ReOrderDates = newReOrderDate != null ? (this.ReOrderDates + "|" + newReOrderDate.ToString()) :
                                                               this.ReOrderDates;
        }

        public List<DateTime> GetReOrderDates()
        {
            List<DateTime> res = new List<DateTime>();
            if (this.ReOrderDates != null)
            {
                string[] ReOrderDates = this.ReOrderDates.Split('|');
                foreach (string date in ReOrderDates)
                {
                    string reasonUI = (date == null || date == "") ? "RAS" : date;
                    res.Add(DateTime.Parse(reasonUI));
                }
            }

            return res;
        }

        public static string GetPrescription(List<CumulSaleAndBillLine> cumulSaleAndBillLines)
        {
            string res = "";

            cumulSaleAndBillLines.Where(csbl => (csbl.Product is Lens) || (csbl.Product is OrderLens)).ToList().ForEach(
                line =>
                {
                    res += line.Product.ProductCode + "<br>";
                });

            return res;
        }


    }
    public enum OriginSaleOperation {Bill, Sale };
}
