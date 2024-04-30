using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
	public class SupplierOrder
		{
		public int SupplierOrderID { get; set; }
		[Index(IsUnique = true)]
        [StringLength(50)]
		public string SupplierOrderReference { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime SupplierOrderDate { get; set; }
        public double VatRate { get; set; }
        public double RateReduction { get; set; }
        public double RateDiscount { get; set; }
        public double Transport { get; set; }
        public bool IsDelivered { get; set; }

		//le fournisseur chez qui on passe la commande
		public int SupplierID { get; set; }
		[ForeignKey("SupplierID")]
		public virtual Supplier Supplier { get; set; }

        ////l'employé de DBOY qui a passé la commande
        //public int OrderedByID { get; set; }
        //[ForeignKey("OrderedByID")]
        //public virtual User OrderedBy { get; set; }
        public int DeviseID { get; set; }
        [ForeignKey("DeviseID")]
        public virtual Devise Devise { get; set; }

        //l'agence qui a passé la commande
        public int BranchID { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }

		public virtual ICollection<SupplierOrderLine> SupplierOrderLines { get; set; }

		[NotMapped]
		public double SupplierOrderTotalAmount { get; set; }

		[NotMapped]
		public String SupplierFullName
			{
			get
				{
                    string itemLabel = this.Supplier.Name +
                           ((this.Supplier.CompanySigle != null && this.Supplier.CompanySigle.Length > 0) ? "" : " " + this.Supplier.Description);
                    return itemLabel;
				}
			}

		[NotMapped]
		public string SupplierEmail
			{
			get
				{
				return this.Supplier.AdressEmail;
				}
			}
		[NotMapped]
		public string SupplierPhoneNumber
			{
			get
				{
				return this.Supplier.AdressPhoneNumber;
				}
			}

        [NotMapped]
        public double SupplierOrderTotalPrice { get; set; }

		}
	}
