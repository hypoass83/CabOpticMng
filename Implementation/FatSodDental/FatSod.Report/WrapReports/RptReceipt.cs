﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class RptReceipt
    {
        public int RptReceiptID { get; set; }
        [Required]
        [StringLength(100)]
        public string Reference { get; set; }
        //Header
        public string Title { get; set; }
        public string BranchName { get; set; }
        public string BranchAdress { get; set; }
        public string BranchTel { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAdress { get; set; }
        public string CompanyTel { get; set; }
        public string CompanyCNI { get; set; }
        public string BranchAbbreviation { get; set; }
        public byte[] CompanyLogo { get; set; }
        //End header=============================
        public DateTime SaleDate { get; set; }
        public string SaleDateUI { get; set; }
        public DateTime SaleDateHours { get; set; }
        public string ProductLabel { get; set; }
        [Required]
        [StringLength(250)]
        public string ProductRef { get; set; }
        public double LineUnitPrice { get; set; }
        public double LineQuantity { get; set; }
        public string MontantLettre { get; set; }
        public string MontantLettreEN { get; set; }
        //===== Customer identification
        public string CustomerName { get; set; }
        public string InsurreName { get; set; }
        public string CustomerAdress { get; set; } //sera utiliser pour l'email de l'agence
        public string CustomerAccount { get; set; }
        //=========
        public double ReceiveAmount { get; set; }
        public double TotalAmount { get; set; }
        public string Operator { get; set; }
        public string DeviseLabel { get; set; }
        public double RateTVA { get; set; }
        public double RateReduction { get; set; }
        public double RateDiscount { get; set; }
        public double Transport { get; set; }
        public int RptReceiptPaymentDetailID { get; set; }

        public double Balance { get; set; }
        public double TotalAmountTTC { get; set; }
        public string Ref { get; set; }

        public virtual ICollection<ReceiptLine> ReceiptLines { get; set; }
        public virtual ICollection<PaymentDetail> PaymentDetails { get; set; }

        public DateTime SaleDeliveryDate { get; set; }
        public string Deliver { get; set; }

        public DateTime OperationDate { get; set; }
        public string OperationHour { get; set; }
        public string BranchEmailAdress { get; set; }
        public string AdressFullName { get; set; }
        public double SommePlafond { get; set; }
        public double SommeTotalMalade { get; set; }
        public double SommeMontantBrut { get; set; }
        public string BranchPhoneNumber { get; set; }
        public string BranchPOBox { get; set; }
        public string CompanyTradeRegister { get; set; }
        public string ONOCNumber { get; set; }
        public string CompanyTown { get; set; }
        public int CompanyLogoID { get; set; }
        public double RemiseAssurance { get; set; }
        public string CompanyRC { get; set; }
    }
}
