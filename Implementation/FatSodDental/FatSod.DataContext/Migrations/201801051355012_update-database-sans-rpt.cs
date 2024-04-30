namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedatabasesansrpt : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.RptAcctingPlans");
            DropTable("dbo.RptBalanceGenerales");
            DropTable("dbo.RptBareCodes");
            DropTable("dbo.RptBills");
            DropTable("dbo.RptBorderoDepotFactures");
            DropTable("dbo.RptbudgetExpenses");
            DropTable("dbo.RptCashDayOperations");
            DropTable("dbo.RptCashOpHists");
            DropTable("dbo.RptCustomerPayments");
            DropTable("dbo.RptEtatsJournals");
            DropTable("dbo.RptHeaders");
            DropTable("dbo.RptIncomeExpenses");
            DropTable("dbo.RptInventoryEntries");
            DropTable("dbo.RptPaymentDetails");
            DropTable("dbo.RptPrintStmts");
            DropTable("dbo.RptPrintStockMvts");
            DropTable("dbo.RptReceipts");
            DropTable("dbo.RptReturnSales");
            DropTable("dbo.RptSpecialOrders");
            DropTable("dbo.RptTransfertForms");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.RptTransfertForms",
                c => new
                    {
                        RptTransfertFormID = c.Int(nullable: false, identity: true),
                        Ref = c.String(),
                        Title = c.String(),
                        BranchName = c.String(),
                        BranchAdress = c.String(),
                        BranchTel = c.String(),
                        CompanyName = c.String(),
                        CompanyAdress = c.String(),
                        CompanyTel = c.String(),
                        CompanyCNI = c.String(),
                        BranchAbbreviation = c.String(),
                        CompanyLogo = c.Binary(),
                        TransfertDate = c.DateTime(nullable: false),
                        ProductLabel = c.String(),
                        ProductRef = c.String(),
                        LineUnitPrice = c.Double(nullable: false),
                        LineQuantity = c.Double(nullable: false),
                        SendindBranchCode = c.String(),
                        SendindBranchName = c.String(),
                        ReceivingBranchCode = c.String(),
                        ReceivingBranchName = c.String(),
                        ReceiveAmount = c.Double(nullable: false),
                        TotalAmount = c.Double(nullable: false),
                        Operator = c.String(),
                        DeviseLabel = c.String(),
                        Transport = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.RptTransfertFormID);
            
            CreateTable(
                "dbo.RptSpecialOrders",
                c => new
                    {
                        RptSpecialOrderID = c.Int(nullable: false, identity: true),
                        Agence = c.String(maxLength: 50),
                        LibAgence = c.String(maxLength: 100),
                        Devise = c.String(maxLength: 3),
                        LibDevise = c.String(maxLength: 100),
                        CustomerOrderDate = c.DateTime(nullable: false),
                        CustomerOrderTotalPrice = c.Double(nullable: false),
                        CustomerOrderNumber = c.String(),
                        CustomerName = c.String(),
                        CodeClient = c.String(nullable: false, maxLength: 100),
                        NomClient = c.String(),
                        OrderStatut = c.String(),
                        Code = c.Int(nullable: false),
                        ValidatedDate = c.DateTime(),
                        DeliveredDate = c.DateTime(),
                        PostedToSupplierDate = c.DateTime(),
                        SaleDate = c.DateTime(),
                        ReceivedDate = c.DateTime(),
                        AdvancedAmount = c.Double(nullable: false),
                        ProductID = c.Int(nullable: false),
                        ProductCode = c.String(),
                        Balance = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.RptSpecialOrderID);
            
            CreateTable(
                "dbo.RptReturnSales",
                c => new
                    {
                        RptReturnSaleID = c.Int(nullable: false, identity: true),
                        Agence = c.String(maxLength: 50),
                        LibAgence = c.String(maxLength: 100),
                        Devise = c.String(maxLength: 5),
                        LibDevise = c.String(maxLength: 100),
                        CodeClient = c.String(maxLength: 50),
                        NomClient = c.String(maxLength: 100),
                        CustomerReturnCauses = c.String(),
                        LineQuantity = c.Double(nullable: false),
                        LineAmount = c.Double(nullable: false),
                        ReturnAmount = c.Double(nullable: false),
                        LocalizationCode = c.String(),
                        ProductCode = c.String(),
                        OeilDroiteGauche = c.String(),
                        CustomerReturnDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RptReturnSaleID);
            
            CreateTable(
                "dbo.RptReceipts",
                c => new
                    {
                        RptReceiptID = c.Int(nullable: false, identity: true),
                        Reference = c.String(nullable: false, maxLength: 100),
                        Title = c.String(),
                        BranchName = c.String(),
                        BranchAdress = c.String(),
                        BranchTel = c.String(),
                        CompanyName = c.String(),
                        CompanyAdress = c.String(),
                        CompanyTel = c.String(),
                        CompanyCNI = c.String(),
                        BranchAbbreviation = c.String(),
                        CompanyLogo = c.Binary(),
                        SaleDate = c.DateTime(nullable: false),
                        ProductLabel = c.String(),
                        ProductRef = c.String(nullable: false, maxLength: 250),
                        LineUnitPrice = c.Double(nullable: false),
                        LineQuantity = c.Double(nullable: false),
                        MontantLettre = c.String(),
                        CustomerName = c.String(),
                        CustomerAdress = c.String(),
                        CustomerAccount = c.String(),
                        ReceiveAmount = c.Double(nullable: false),
                        TotalAmount = c.Double(nullable: false),
                        Operator = c.String(),
                        DeviseLabel = c.String(),
                        RateTVA = c.Double(nullable: false),
                        RateReduction = c.Double(nullable: false),
                        RateDiscount = c.Double(nullable: false),
                        Transport = c.Double(nullable: false),
                        RptReceiptPaymentDetailID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RptReceiptID);
            
            CreateTable(
                "dbo.RptPrintStockMvts",
                c => new
                    {
                        RptPrintStockMvtID = c.Int(nullable: false, identity: true),
                        Agence = c.String(maxLength: 50),
                        LibAgence = c.String(maxLength: 100),
                        LocalizationName = c.String(maxLength: 100),
                        ProductName = c.String(maxLength: 500),
                        Devise = c.String(maxLength: 3),
                        LibDevise = c.String(maxLength: 100),
                        EndDate = c.DateTime(nullable: false),
                        BeginDate = c.DateTime(nullable: false),
                        DateOperation = c.DateTime(nullable: false),
                        RefOperation = c.String(maxLength: 30),
                        Description = c.String(maxLength: 100),
                        RepOutPut = c.Double(nullable: false),
                        RepInput = c.Double(nullable: false),
                        Solde = c.Double(nullable: false),
                        QteOutPut = c.Double(nullable: false),
                        QteInput = c.Double(nullable: false),
                        Sens = c.String(maxLength: 10),
                        CompanyName = c.String(maxLength: 255),
                        RegionCountry = c.String(maxLength: 255),
                        Telephone = c.String(maxLength: 30),
                        Fax = c.String(maxLength: 30),
                        Adresse = c.String(maxLength: 255),
                        LogoBranch = c.Binary(),
                        ProductID = c.Int(nullable: false),
                        LocalizationID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RptPrintStockMvtID);
            
            CreateTable(
                "dbo.RptPrintStmts",
                c => new
                    {
                        RptPrintStmtID = c.Int(nullable: false, identity: true),
                        Agence = c.String(maxLength: 50),
                        LibAgence = c.String(maxLength: 100),
                        AcctNo = c.String(maxLength: 10),
                        AcctName = c.String(maxLength: 100),
                        Devise = c.String(maxLength: 3),
                        LibDevise = c.String(maxLength: 100),
                        EndDate = c.DateTime(nullable: false),
                        BeginDate = c.DateTime(nullable: false),
                        DateOperation = c.DateTime(nullable: false),
                        RefOperation = c.String(maxLength: 30),
                        Description = c.String(maxLength: 100),
                        RepDebit = c.Double(nullable: false),
                        RepCredit = c.Double(nullable: false),
                        Solde = c.Double(nullable: false),
                        MtDebit = c.Double(nullable: false),
                        MtCredit = c.Double(nullable: false),
                        Sens = c.String(maxLength: 3),
                        CompanyName = c.String(maxLength: 255),
                        RegionCountry = c.String(maxLength: 255),
                        Telephone = c.String(maxLength: 30),
                        Fax = c.String(maxLength: 30),
                        Adresse = c.String(maxLength: 255),
                        LogoBranch = c.Binary(),
                        SaleID = c.Int(),
                    })
                .PrimaryKey(t => t.RptPrintStmtID);
            
            CreateTable(
                "dbo.RptPaymentDetails",
                c => new
                    {
                        RptPaymentDetailID = c.Int(nullable: false, identity: true),
                        Reference = c.String(nullable: false, maxLength: 100),
                        DepositDate = c.DateTime(nullable: false),
                        Description = c.String(),
                        LineUnitPrice = c.Double(nullable: false),
                        RptReceiptPaymentDetailID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RptPaymentDetailID);
            
            CreateTable(
                "dbo.RptInventoryEntries",
                c => new
                    {
                        RptInventoryEntryID = c.Int(nullable: false, identity: true),
                        Ref = c.String(),
                        Title = c.String(),
                        BranchName = c.String(),
                        BranchAdress = c.String(),
                        BranchTel = c.String(),
                        CompanyName = c.String(),
                        CompanyAdress = c.String(),
                        CompanyTel = c.String(),
                        CompanyCNI = c.String(),
                        CompanyLogo = c.Binary(),
                        InventoryDirectoryDate = c.DateTime(nullable: false),
                        ProductLabel = c.String(),
                        ProductRef = c.String(),
                        StockDifference = c.Double(nullable: false),
                        OldStockQuantity = c.Double(nullable: false),
                        NewStockQuantity = c.Double(nullable: false),
                        AveragePurchasePrice = c.Double(nullable: false),
                        Operator = c.String(),
                        DeviseLabel = c.String(),
                    })
                .PrimaryKey(t => t.RptInventoryEntryID);
            
            CreateTable(
                "dbo.RptIncomeExpenses",
                c => new
                    {
                        RptIncomeExpenseID = c.Int(nullable: false, identity: true),
                        Agence = c.String(maxLength: 50),
                        LibAgence = c.String(maxLength: 100),
                        Devise = c.String(maxLength: 50),
                        LibDevise = c.String(maxLength: 10),
                        AcctNumber = c.String(maxLength: 10),
                        AcctName = c.String(maxLength: 100),
                        MonthTotal = c.Double(nullable: false),
                        MonthCumul = c.Double(nullable: false),
                        earningsmonth = c.Double(nullable: false),
                        earningscumul = c.Double(nullable: false),
                        AccountType = c.String(maxLength: 25),
                    })
                .PrimaryKey(t => t.RptIncomeExpenseID);
            
            CreateTable(
                "dbo.RptHeaders",
                c => new
                    {
                        RptHeaderID = c.Int(nullable: false, identity: true),
                        CompanySigle = c.String(),
                        CompanyTradeRegister = c.String(),
                        CompanySlogan = c.String(),
                        CompanyName = c.String(),
                        CompanyAdress = c.String(),
                        CompanyTel = c.String(),
                        CompanyFax = c.String(),
                        CompanyEmail = c.String(),
                        CompanyWebSite = c.String(),
                        CompanyLogo = c.Binary(),
                        BranchName = c.String(),
                        BranchCode = c.String(),
                        BranchAdress = c.String(),
                        BranchTel = c.String(),
                        BranchFax = c.String(),
                        BranchEmail = c.String(),
                    })
                .PrimaryKey(t => t.RptHeaderID);
            
            CreateTable(
                "dbo.RptEtatsJournals",
                c => new
                    {
                        RptEtatsJournalID = c.Int(nullable: false, identity: true),
                        Agence = c.String(maxLength: 50),
                        LibAgence = c.String(maxLength: 100),
                        Devise = c.String(maxLength: 3),
                        LibDevise = c.String(maxLength: 100),
                        CompteCle = c.String(maxLength: 12),
                        LibelleCpte = c.String(maxLength: 100),
                        CodeOperation = c.String(maxLength: 30),
                        LibelleOperation = c.String(maxLength: 100),
                        Reference = c.String(maxLength: 50),
                        Desription = c.String(maxLength: 100),
                        DateOperation = c.DateTime(nullable: false),
                        MontantDB = c.Double(nullable: false),
                        MontantCR = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.RptEtatsJournalID);
            
            CreateTable(
                "dbo.RptCustomerPayments",
                c => new
                    {
                        RptCustomerPaymentID = c.Int(nullable: false, identity: true),
                        Date = c.String(),
                        Type = c.String(),
                        RptTitle = c.String(),
                        Solde = c.Double(nullable: false),
                        TillOpeningAmoung = c.Double(nullable: false),
                        InputAmount = c.Double(nullable: false),
                        OutPutAmount = c.Double(nullable: false),
                        Teller = c.String(),
                        TransactionNumber = c.String(),
                        CashRegisterName = c.String(),
                        Intervenant = c.String(),
                        Operation = c.String(),
                        BranchName = c.String(),
                        BranchAdress = c.String(),
                        BranchTel = c.String(),
                        CompanyName = c.String(),
                        CompanyAdress = c.String(),
                        CompanyTel = c.String(),
                        CompanyCNI = c.String(),
                        CompanyLogo = c.Binary(),
                    })
                .PrimaryKey(t => t.RptCustomerPaymentID);
            
            CreateTable(
                "dbo.RptCashOpHists",
                c => new
                    {
                        RptCashOpHistID = c.Int(nullable: false, identity: true),
                        OperationDate = c.DateTime(nullable: false),
                        BeginDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        RptTitle = c.String(),
                        Solde = c.Double(nullable: false),
                        GroupingDate = c.Double(nullable: false),
                        InputAmount = c.Double(nullable: false),
                        RealOperationAmount = c.Double(nullable: false),
                        OutPutAmount = c.Double(nullable: false),
                        OpeningCashAmount = c.Double(nullable: false),
                        ClosingCashAmount = c.Double(nullable: false),
                        Teller = c.String(),
                        TransactionNumber = c.String(nullable: false, maxLength: 100),
                        CashRegisterName = c.String(),
                        Intervenant = c.String(),
                        Operation = c.String(),
                        BranchName = c.String(),
                        BranchAdress = c.String(),
                        BranchTel = c.String(),
                        CompanyName = c.String(),
                        CompanyAdress = c.String(),
                        CompanyTel = c.String(),
                        CompanyCNI = c.String(),
                        CompanyLogo = c.Binary(),
                        PaymentMethod = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RptCashOpHistID);
            
            CreateTable(
                "dbo.RptCashDayOperations",
                c => new
                    {
                        RptCashDayOperationID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Type = c.String(),
                        RptTitle = c.String(),
                        Solde = c.Double(nullable: false),
                        RealOperationAmount = c.Double(nullable: false),
                        TillOpeningAmoung = c.Double(nullable: false),
                        InputAmount = c.Double(nullable: false),
                        OutPutAmount = c.Double(nullable: false),
                        Teller = c.String(),
                        TransactionNumber = c.String(),
                        CashRegisterName = c.String(),
                        Intervenant = c.String(),
                        Operation = c.String(),
                        BranchName = c.String(),
                        BranchAdress = c.String(),
                        BranchTel = c.String(),
                        CompanyName = c.String(),
                        CompanyAdress = c.String(),
                        CompanyTel = c.String(),
                        CompanyCNI = c.String(),
                        CompanyLogo = c.Binary(),
                    })
                .PrimaryKey(t => t.RptCashDayOperationID);
            
            CreateTable(
                "dbo.RptbudgetExpenses",
                c => new
                    {
                        RptbudgetExpenseID = c.Int(nullable: false, identity: true),
                        Agence = c.String(maxLength: 50),
                        LibAgence = c.String(maxLength: 100),
                        Devise = c.String(),
                        LibDevise = c.String(maxLength: 100),
                        UIBudgetAllocated = c.String(maxLength: 100),
                        PaymentMethodId = c.Int(nullable: false),
                        VoucherAmount = c.Double(nullable: false),
                        DateOperation = c.DateTime(nullable: false),
                        Reference = c.String(maxLength: 30),
                        BeneficiaryName = c.String(maxLength: 100),
                        Justification = c.String(maxLength: 100),
                        BudgetConsumptionID = c.Int(nullable: false),
                        PaymentDate = c.DateTime(nullable: false),
                        CompanyName = c.String(maxLength: 255),
                        RegionCountry = c.String(maxLength: 255),
                        Telephone = c.String(maxLength: 30),
                        Fax = c.String(maxLength: 30),
                        Adresse = c.String(maxLength: 255),
                        LogoBranch = c.Binary(),
                    })
                .PrimaryKey(t => t.RptbudgetExpenseID);
            
            CreateTable(
                "dbo.RptBorderoDepotFactures",
                c => new
                    {
                        RptBorderoDepotFactureID = c.Int(nullable: false, identity: true),
                        CustomerOrderID = c.Int(nullable: false),
                        BranchID = c.Int(nullable: false),
                        UIBranchCode = c.String(maxLength: 100),
                        CustomerName = c.String(maxLength: 100),
                        CompanyName = c.String(maxLength: 100),
                        CustomerOrderNumber = c.String(maxLength: 50),
                        NumeroBonPriseEnCharge = c.String(maxLength: 100),
                        CustomerOrderDate = c.DateTime(nullable: false),
                        NumeroFacture = c.String(maxLength: 30),
                        PhoneNumber = c.String(maxLength: 30),
                        MntAssureur = c.Double(nullable: false),
                        ReductionAmount = c.Double(nullable: false),
                        LogoBranch = c.Binary(),
                        InsuranceCompany = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.RptBorderoDepotFactureID);
            
            CreateTable(
                "dbo.RptBills",
                c => new
                    {
                        RptBillID = c.Int(nullable: false, identity: true),
                        Ref = c.String(),
                        Title = c.String(),
                        BranchName = c.String(),
                        BranchAdress = c.String(),
                        BranchTel = c.String(),
                        CompanyName = c.String(),
                        CompanyAdress = c.String(),
                        CompanyTel = c.String(),
                        CompanyCNI = c.String(),
                        CompanyLogo = c.Binary(),
                        BranchAbbreviation = c.String(),
                        SaleDate = c.DateTime(nullable: false),
                        RateRedution = c.Double(nullable: false),
                        RateDiscount = c.Double(nullable: false),
                        Redution = c.Double(nullable: false),
                        Discount = c.Double(nullable: false),
                        Transport = c.Double(nullable: false),
                        ProductLabel = c.String(),
                        ProductRef = c.String(),
                        LineUnitPrice = c.Double(nullable: false),
                        LineQuantity = c.Double(nullable: false),
                        ReceiveAmount = c.Double(nullable: false),
                        SaleID = c.Int(nullable: false),
                        CustomerName = c.String(),
                        CustomerAdress = c.String(),
                        CustomerAccount = c.String(),
                        VatRate = c.Double(nullable: false),
                        TotalRemainingUnpaid = c.Double(nullable: false),
                        CompanyEmail = c.String(),
                        CompanyTradeRegister = c.String(),
                        DepositAmount = c.Double(nullable: false),
                        CompanyTown = c.String(),
                        BillNumber = c.String(),
                    })
                .PrimaryKey(t => t.RptBillID);
            
            CreateTable(
                "dbo.RptBareCodes",
                c => new
                    {
                        RptBareCodeID = c.Int(nullable: false, identity: true),
                        BareCode = c.String(maxLength: 10),
                        ProductName = c.String(maxLength: 100),
                        ProductDescription = c.String(maxLength: 100),
                        Price = c.Double(nullable: false),
                        BarcodeImage = c.Binary(),
                    })
                .PrimaryKey(t => t.RptBareCodeID);
            
            CreateTable(
                "dbo.RptBalanceGenerales",
                c => new
                    {
                        RptBalanceGeneraleID = c.Int(nullable: false, identity: true),
                        Agence = c.String(maxLength: 50),
                        LibAgence = c.String(maxLength: 100),
                        Devise = c.String(maxLength: 5),
                        LibDevise = c.String(maxLength: 100),
                        Compte = c.String(maxLength: 50),
                        Libelle = c.String(maxLength: 100),
                        SoldeInitDb = c.Double(nullable: false),
                        SoldeInitCr = c.Double(nullable: false),
                        DebitMvt = c.Double(nullable: false),
                        CreditMvt = c.Double(nullable: false),
                        DebitCum = c.Double(nullable: false),
                        CreditCum = c.Double(nullable: false),
                        SoldeFinDb = c.Double(nullable: false),
                        SoldeFinCr = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.RptBalanceGeneraleID);
            
            CreateTable(
                "dbo.RptAcctingPlans",
                c => new
                    {
                        RptAcctingPlanID = c.Int(nullable: false, identity: true),
                        CompteCle = c.String(maxLength: 10),
                        LibelleCpte = c.String(maxLength: 100),
                        ManualPosting = c.Boolean(nullable: false),
                        Devise = c.String(maxLength: 3),
                    })
                .PrimaryKey(t => t.RptAcctingPlanID);
            
        }
    }
}
