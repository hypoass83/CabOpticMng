using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FatSod.Security.Entities;

namespace FatSod.Supply.Entities
{
    public class CustomerComplaint
    {
        public int CustomerComplaintId { get; set; }
        public string Complaint { get; set; }
        public string ResolverComment { get; set; }
        public string ControllerComment { get; set; }
        public int Occurrences { get; set; }

        public DateTime RegistrationDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public DateTime? ControlledDate { get; set; }

        public int? CumulSaleAndBillID { get; set; }
        [ForeignKey("CumulSaleAndBillID")]
        public virtual CumulSaleAndBill CumulSaleAndBill { get; set; }

        /// <summary>
        /// C'est le profile de l'utilisateur qui peut traiter la plainte ou alors
        /// qui peut voir la plainte.
        /// </summary>
        public int ComplaintQuotationId { get; set; }
        [ForeignKey("ComplaintQuotationId")]
        public virtual Profile ComplaintQuotation { get; set; }

        /// <summary>
        /// C'est la personne physique qui a resolu le probleme du client
        /// </summary>
        public int? ComplaintResolverId { get; set; }
        [ForeignKey("ComplaintResolverId")]
        public virtual User ComplaintResolver { get; set; }

        #region Champs important si le client est introuvable dans le tableau
        public string Customer { get; set; }
        public string PhoneNumber { get; set; }
        public string Insurance { get; set; }
        public string InsuredCompany { get; set; }
        public bool IsCashCustomer { get; set; }
        public bool IsCashOtherCustomer { get; set; }
        public bool IsInsuredCustomer { get; set; }
        public DateTime? PurchaseDate { get; set; }
        #endregion
        public void AddResolverComment(string comment, string resolverName, string date)
        {
            string newComment = /*"<strong>"* +*/ resolverName + " ( " + date + " )" + "\n" /*"</strong><br/>"*/;
            newComment += comment;
            this.ResolverComment += newComment + "\n \n";
        }
        /// <summary>
        /// C'est celui qui a controller ce qu'a fait le Resolver(<see cref="ComplaintResolver"/>)
        /// </summary>
        public int? ComplaintControllerId { get; set; }
        [ForeignKey("ComplaintControllerId")]
        public virtual User ComplaintController { get; set; }
        [NotMapped]
        public string State { get; set; }
        [NotMapped]
        public string Quotation { get; set; }
        [NotMapped]
        public string Resolver { get; set; }
        [NotMapped]
        public string Controller { get; set; }
        [NotMapped]
        public string DisplayDate { get; set; }
        [NotMapped]
        public string DisplaySaleDate { get; set; }
        [NotMapped]
        public string CustomerType { get; set; }
        [NotMapped]
        public string CustomerValue { get; set; }
        [NotMapped]
        public bool IsCustomerNotFound { get; set; }
        [NotMapped]
        public string PreviousComment { get; set; }
        [NotMapped]
        public bool IsSolved { get; set; }

    }
}
