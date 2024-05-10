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
	public class Line
	{
		public int LineID { get; set; }
		public double LineUnitPrice { get; set; }
		public double LineQuantity { get; set; }
		public int LocalizationID { get; set; }
		[ForeignKey("LocalizationID")]
		public virtual Localization Localization { get; set; }
		public int ProductID { get; set; }
		[ForeignKey("ProductID")]
		public virtual Product Product { get; set; }
		public bool isPost { get; set; }
		public EyeSide OeilDroiteGauche { get; set; } //oeil droite gauche
        public string SupplyingName { get; set; }
        public bool isGift { get; set; }
        public bool isCommandGlass { get; set; }
        public string NumeroSerie { get; set; }
		// pour dire si oui(1) ou non(-1) un cadre vient de la salle VIP
		public int IsVIPRoom { get; set; }

		//propriétés non persistantes

		[NotMapped]
		public double SafetyStockQuantity { get; set; }

		[NotMapped]
		public int TMPID { get; set; }

		[NotMapped]
		public double LineAmount
		{
			get
			{
				return LineQuantity * LineUnitPrice;
			}
		}

		[NotMapped]
		public String ProductLabel
		{
			get
			{
				return this.Product.ProductCode;
			}
		}

		[NotMapped]
		public String LocalizationLabel
		{
			get
			{
				return this.Localization.LocalizationLabel;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null) { return false; }
			if (!(obj is Line)) { return false; }
			Line l = (Line)obj;
			return this.ProductID == l.ProductID &&
						 this.LocalizationID == l.LocalizationID &&
						 this.TMPID == l.TMPID
					;
		}

		public bool Exist(object obj)
		{
			if (obj == null) { return false; }
			if (!(obj is Line)) { return false; }
			Line l = (Line)obj;
			return this.ProductID == l.ProductID &&
						 this.LocalizationID == l.LocalizationID
					;
		}
	}
	public enum EyeSide
	{
		OD,
		OG,
		ODG,
		N
	}
}
