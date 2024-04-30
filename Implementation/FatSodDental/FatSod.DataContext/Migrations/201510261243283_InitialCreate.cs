namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountingSections",
                c => new
                    {
                        AccountingSectionID = c.Int(nullable: false, identity: true),
                        AccountingSectionNumber = c.Int(nullable: false),
                        AccountingSectionCode = c.String(nullable: false, maxLength: 100),
                        AccountingSectionLabel = c.String(),
                        ClassAccountID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountingSectionID)
                .ForeignKey("dbo.ClassAccounts", t => t.ClassAccountID)
                .Index(t => t.AccountingSectionNumber, unique: true)
                .Index(t => t.AccountingSectionCode, unique: true, name: "CategoryCode")
                .Index(t => t.ClassAccountID);
            
            CreateTable(
                "dbo.AccountingTask",
                c => new
                    {
                        AccountingTaskID = c.Int(nullable: false, identity: true),
                        AccountingTaskSens = c.String(),
                        AccountingTaskDescription = c.String(),
                        OperationID = c.Int(nullable: false),
                        AccountingSectionID = c.Int(nullable: false),
                        ApplyVat = c.String(),
                        Account_AccountID = c.Int(),
                    })
                .PrimaryKey(t => t.AccountingTaskID)
                .ForeignKey("dbo.Accounts", t => t.Account_AccountID)
                .ForeignKey("dbo.Operations", t => t.OperationID)
                .ForeignKey("dbo.AccountingSections", t => t.AccountingSectionID)
                .Index(t => t.OperationID)
                .Index(t => t.AccountingSectionID)
                .Index(t => t.Account_AccountID);
            
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        AccountID = c.Int(nullable: false, identity: true),
                        AccountNumber = c.Int(nullable: false),
                        AccountLabel = c.String(),
                        isManualPosting = c.Boolean(nullable: false),
                        CollectifAccountID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountID)
                .ForeignKey("dbo.CollectifAccounts", t => t.CollectifAccountID)
                .Index(t => t.AccountNumber, unique: true)
                .Index(t => t.CollectifAccountID);
            
            CreateTable(
                "dbo.AccountOperations",
                c => new
                    {
                        AccountOperationID = c.Long(nullable: false, identity: true),
                        BranchID = c.Int(nullable: false),
                        OperationID = c.Int(nullable: false),
                        AccountID = c.Int(nullable: false),
                        AccountTierID = c.Int(),
                        DeviseID = c.Int(nullable: false),
                        DateOperation = c.DateTime(nullable: false),
                        Description = c.String(),
                        Reference = c.String(),
                        CodeTransaction = c.String(),
                        Debit = c.Double(nullable: false),
                        Credit = c.Double(nullable: false),
                        TillAdjustID = c.Int(),
                        Discriminator = c.String(maxLength: 128),
                        Account_AccountID = c.Int(),
                    })
                .PrimaryKey(t => t.AccountOperationID)
                .ForeignKey("dbo.Accounts", t => t.AccountID)
                .ForeignKey("dbo.Devises", t => t.DeviseID)
                .ForeignKey("dbo.Operations", t => t.OperationID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .ForeignKey("dbo.TillAdjusts", t => t.TillAdjustID)
                .ForeignKey("dbo.Accounts", t => t.Account_AccountID)
                .Index(t => t.BranchID)
                .Index(t => t.OperationID)
                .Index(t => t.AccountID)
                .Index(t => t.DeviseID)
                .Index(t => t.TillAdjustID)
                .Index(t => t.Account_AccountID);
            
            CreateTable(
                "dbo.Branches",
                c => new
                    {
                        BranchID = c.Int(nullable: false, identity: true),
                        BranchCode = c.String(nullable: false, maxLength: 100),
                        BranchName = c.String(nullable: false, maxLength: 100),
                        BranchDescription = c.String(),
                        AdressID = c.Int(nullable: false),
                        CompanyID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BranchID)
                .ForeignKey("dbo.Companies", t => t.CompanyID)
                .ForeignKey("dbo.Adresses", t => t.AdressID)
                .Index(t => t.BranchCode, unique: true, name: "BranchCode")
                .Index(t => t.BranchName, unique: true, name: "BranchName")
                .Index(t => t.AdressID)
                .Index(t => t.CompanyID);
            
            CreateTable(
                "dbo.Adresses",
                c => new
                    {
                        AdressID = c.Int(nullable: false, identity: true),
                        AdressPhoneNumber = c.String(),
                        AdressCellNumber = c.String(),
                        AdressFullName = c.String(),
                        AdressEmail = c.String(),
                        AdressWebSite = c.String(),
                        AdressPOBox = c.String(),
                        AdressFax = c.String(),
                        QuarterID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AdressID)
                .ForeignKey("dbo.Quarters", t => t.QuarterID)
                .Index(t => t.QuarterID);
            
            CreateTable(
                "dbo.GlobalPeople",
                c => new
                    {
                        GlobalPersonID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Tiergroup = c.String(),
                        Description = c.String(),
                        CNI = c.String(nullable: false, maxLength: 100),
                        AdressID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GlobalPersonID)
                .ForeignKey("dbo.Adresses", t => t.AdressID)
                .Index(t => t.CNI, unique: true, name: "CNI")
                .Index(t => t.AdressID);
            
            CreateTable(
                "dbo.Files",
                c => new
                    {
                        FileID = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        ContentType = c.String(),
                        Content = c.Binary(),
                        FileType = c.Int(nullable: false),
                        GlobalPersonID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FileID)
                .ForeignKey("dbo.GlobalPeople", t => t.GlobalPersonID)
                .Index(t => t.GlobalPersonID);
            
            CreateTable(
                "dbo.Jobs",
                c => new
                    {
                        JobID = c.Int(nullable: false, identity: true),
                        JobLabel = c.String(),
                        JobDescription = c.String(),
                        JobCode = c.String(),
                        CompanyID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.JobID)
                .ForeignKey("dbo.Companies", t => t.CompanyID)
                .Index(t => t.CompanyID);
            
            CreateTable(
                "dbo.Sexes",
                c => new
                    {
                        SexID = c.Int(nullable: false, identity: true),
                        SexCode = c.String(maxLength: 100),
                        SexLabel = c.String(),
                    })
                .PrimaryKey(t => t.SexID)
                .Index(t => t.SexCode, unique: true);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        ProfileID = c.Int(nullable: false, identity: true),
                        ProfileCode = c.String(nullable: false, maxLength: 100),
                        ProfileLabel = c.String(),
                        ProfileDescription = c.String(),
                        ProfileState = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ProfileID)
                .Index(t => t.ProfileCode, unique: true, name: "ProfileCode");
            
            CreateTable(
                "dbo.ActionMenuProfiles",
                c => new
                    {
                        ActionMenuProfileID = c.Int(nullable: false, identity: true),
                        Delete = c.Boolean(nullable: false),
                        Add = c.Boolean(nullable: false),
                        Update = c.Boolean(nullable: false),
                        MenuID = c.Int(nullable: false),
                        ProfileID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ActionMenuProfileID)
                .ForeignKey("dbo.Menus", t => t.MenuID)
                .ForeignKey("dbo.Profiles", t => t.ProfileID)
                .Index(t => new { t.MenuID, t.ProfileID }, unique: true, name: "IX_RealPrimaryKey");
            
            CreateTable(
                "dbo.Menus",
                c => new
                    {
                        MenuID = c.Int(nullable: false, identity: true),
                        MenuCode = c.String(nullable: false, maxLength: 100),
                        MenuDescription = c.String(),
                        MenuController = c.String(),
                        MenuState = c.Boolean(nullable: false),
                        MenuLabel = c.String(),
                        MenuFlat = c.Boolean(nullable: false),
                        MenuIconName = c.String(),
                        MenuPath = c.String(),
                        IsChortcut = c.Boolean(nullable: false),
                        ModuleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MenuID)
                .ForeignKey("dbo.Modules", t => t.ModuleID)
                .Index(t => t.MenuCode, unique: true, name: "MenuCode")
                .Index(t => t.ModuleID);
            
            CreateTable(
                "dbo.Modules",
                c => new
                    {
                        ModuleID = c.Int(nullable: false, identity: true),
                        ModuleCode = c.String(nullable: false, maxLength: 100),
                        ModuleLabel = c.String(),
                        ModuleDescription = c.String(),
                        ModuleImagePath = c.String(),
                        ModulePressedImagePath = c.String(),
                        ModuleDisabledImagePath = c.String(),
                        ModuleArea = c.String(),
                        ModuleImageHeight = c.Int(nullable: false),
                        ModuleImageWeight = c.Int(nullable: false),
                        ModuleState = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ModuleID)
                .Index(t => t.ModuleCode, unique: true, name: "ModuleCode");
            
            CreateTable(
                "dbo.SubMenus",
                c => new
                    {
                        SubMenuID = c.Int(nullable: false, identity: true),
                        SubMenuCode = c.String(nullable: false, maxLength: 100),
                        SubMenuLabel = c.String(),
                        SubMenuDescription = c.String(),
                        SubMenuController = c.String(),
                        SubMenuPath = c.String(),
                        IsChortcut = c.Boolean(nullable: false),
                        MenuID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SubMenuID)
                .ForeignKey("dbo.Menus", t => t.MenuID)
                .Index(t => t.SubMenuCode, unique: true, name: "SubMenuCode")
                .Index(t => t.MenuID);
            
            CreateTable(
                "dbo.ActionSubMenuProfiles",
                c => new
                    {
                        ActionSubMenuProfileID = c.Int(nullable: false, identity: true),
                        Delete = c.Boolean(nullable: false),
                        Add = c.Boolean(nullable: false),
                        Update = c.Boolean(nullable: false),
                        SubMenuID = c.Int(nullable: false),
                        ProfileID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ActionSubMenuProfileID)
                .ForeignKey("dbo.Profiles", t => t.ProfileID)
                .ForeignKey("dbo.SubMenus", t => t.SubMenuID)
                .Index(t => new { t.SubMenuID, t.ProfileID }, unique: true, name: "IX_RealPrimaryKey");
            
            CreateTable(
                "dbo.UserBranches",
                c => new
                    {
                        UserBranchID = c.Int(nullable: false, identity: true),
                        IsDeleted = c.Boolean(nullable: false),
                        BranchID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserBranchID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .Index(t => t.BranchID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.UserConfigurations",
                c => new
                    {
                        UserConfigurationID = c.Int(nullable: false, identity: true),
                        DefaultCulture = c.String(),
                        DefaultBranchID = c.Int(nullable: false),
                        DefaultDeviseID = c.Int(nullable: false),
                        DefaultLocationID = c.Int(),
                    })
                .PrimaryKey(t => t.UserConfigurationID)
                .ForeignKey("dbo.Branches", t => t.DefaultBranchID)
                .Index(t => t.DefaultBranchID);
            
            CreateTable(
                "dbo.CustomerOrders",
                c => new
                    {
                        CustomerOrderID = c.Int(nullable: false, identity: true),
                        CustomerOrderDate = c.DateTime(nullable: false),
                        VatRate = c.Double(nullable: false),
                        RateReduction = c.Double(nullable: false),
                        RateDiscount = c.Double(nullable: false),
                        Patient = c.String(),
                        CustomerOrderNumber = c.String(maxLength: 100),
                        IsDelivered = c.Boolean(nullable: false),
                        CustomerID = c.Int(nullable: false),
                        DeviseID = c.Int(nullable: false),
                        BranchID = c.Int(nullable: false),
                        OperatorID = c.Int(nullable: false),
                        Transport = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerOrderID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .ForeignKey("dbo.Customers", t => t.CustomerID)
                .ForeignKey("dbo.Devises", t => t.DeviseID)
                .ForeignKey("dbo.Users", t => t.OperatorID)
                .Index(t => t.CustomerOrderNumber, unique: true)
                .Index(t => t.CustomerID)
                .Index(t => t.DeviseID)
                .Index(t => t.BranchID)
                .Index(t => t.OperatorID);
            
            CreateTable(
                "dbo.Lines",
                c => new
                    {
                        LineID = c.Int(nullable: false, identity: true),
                        LineUnitPrice = c.Double(nullable: false),
                        LineQuantity = c.Double(nullable: false),
                        LocalizationID = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        isPost = c.Boolean(nullable: false),
                        OeilDroiteGauche = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LineID)
                .ForeignKey("dbo.Localizations", t => t.LocalizationID)
                .ForeignKey("dbo.Products", t => t.ProductID)
                .Index(t => t.LocalizationID)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.Localizations",
                c => new
                    {
                        LocalizationID = c.Int(nullable: false, identity: true),
                        LocalizationCode = c.String(maxLength: 100),
                        LocalizationLabel = c.String(),
                        LocalizationDescription = c.String(),
                        QuarterID = c.Int(nullable: false),
                        BranchID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LocalizationID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .ForeignKey("dbo.Quarters", t => t.QuarterID)
                .Index(t => t.LocalizationCode, unique: true)
                .Index(t => t.QuarterID)
                .Index(t => t.BranchID);
            
            CreateTable(
                "dbo.ProductLocalizations",
                c => new
                    {
                        ProductLocalizationID = c.Int(nullable: false, identity: true),
                        ProductLocalizationStockQuantity = c.Double(nullable: false),
                        ProductLocalizationSafetyStockQuantity = c.Double(nullable: false),
                        ProductLocalizationStockSellingPrice = c.Double(nullable: false),
                        AveragePurchasePrice = c.Double(nullable: false),
                        ProductLocalizationDate = c.DateTime(nullable: false),
                        ProductID = c.Int(nullable: false),
                        LocalizationID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductLocalizationID)
                .ForeignKey("dbo.Localizations", t => t.LocalizationID)
                .ForeignKey("dbo.Products", t => t.ProductID)
                .Index(t => new { t.ProductID, t.LocalizationID }, unique: true, name: "IX_RealPrimaryKey");
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductID = c.Int(nullable: false, identity: true),
                        ProductCode = c.String(nullable: false, maxLength: 100),
                        ProductLabel = c.String(),
                        ProductDescription = c.String(),
                        CategoryID = c.Int(nullable: false),
                        AccountID = c.Int(nullable: false),
                        SellingPrice = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ProductID)
                .ForeignKey("dbo.Accounts", t => t.AccountID)
                .ForeignKey("dbo.Categories", t => t.CategoryID)
                .Index(t => t.ProductCode, unique: true, name: "ProductCode")
                .Index(t => t.CategoryID)
                .Index(t => t.AccountID);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryID = c.Int(nullable: false, identity: true),
                        CategoryCode = c.String(nullable: false, maxLength: 100),
                        CategoryLabel = c.String(),
                        CategoryDescription = c.String(),
                    })
                .PrimaryKey(t => t.CategoryID)
                .Index(t => t.CategoryCode, unique: true, name: "CategoryCode");
            
            CreateTable(
                "dbo.CollectifAccounts",
                c => new
                    {
                        CollectifAccountID = c.Int(nullable: false, identity: true),
                        CollectifAccountNumber = c.Int(nullable: false),
                        CollectifAccountLabel = c.String(),
                        AccountingSectionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CollectifAccountID)
                .ForeignKey("dbo.AccountingSections", t => t.AccountingSectionID)
                .Index(t => t.CollectifAccountNumber, unique: true)
                .Index(t => t.AccountingSectionID);
            
            CreateTable(
                "dbo.LensCoatings",
                c => new
                    {
                        LensCoatingID = c.Int(nullable: false, identity: true),
                        LensCoatingCode = c.String(nullable: false, maxLength: 100),
                        LensCoatingLabel = c.String(),
                        LensCoatingDescription = c.String(),
                    })
                .PrimaryKey(t => t.LensCoatingID)
                .Index(t => t.LensCoatingCode, unique: true, name: "LensCoatingCode");
            
            CreateTable(
                "dbo.LensColours",
                c => new
                    {
                        LensColourID = c.Int(nullable: false, identity: true),
                        LensColourCode = c.String(nullable: false, maxLength: 100),
                        LensColourLabel = c.String(),
                        LensColourDescription = c.String(),
                    })
                .PrimaryKey(t => t.LensColourID)
                .Index(t => t.LensColourCode, unique: true, name: "LensColourCode");
            
            CreateTable(
                "dbo.LensMaterials",
                c => new
                    {
                        LensMaterialID = c.Int(nullable: false, identity: true),
                        LensMaterialCode = c.String(nullable: false, maxLength: 100),
                        LensMaterialLabel = c.String(),
                        LensMaterialDescription = c.String(),
                    })
                .PrimaryKey(t => t.LensMaterialID)
                .Index(t => t.LensMaterialCode, unique: true, name: "LensMaterialCode");
            
            CreateTable(
                "dbo.LensNumbers",
                c => new
                    {
                        LensNumberID = c.Int(nullable: false, identity: true),
                        LensNumberSphericalValue = c.String(maxLength: 10),
                        LensNumberCylindricalValue = c.String(maxLength: 10),
                        LensNumberAdditionValue = c.String(maxLength: 10),
                        LensNumberDescription = c.String(),
                    })
                .PrimaryKey(t => t.LensNumberID)
                .Index(t => new { t.LensNumberSphericalValue, t.LensNumberCylindricalValue, t.LensNumberAdditionValue }, unique: true, name: "IX_RealPrimaryKey");
            
            CreateTable(
                "dbo.Quarters",
                c => new
                    {
                        QuarterID = c.Int(nullable: false, identity: true),
                        QuarterCode = c.String(maxLength: 100),
                        QuarterLabel = c.String(),
                        TownID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QuarterID)
                .ForeignKey("dbo.Towns", t => t.TownID)
                .Index(t => t.QuarterCode, unique: true, name: "QuarterCode")
                .Index(t => t.TownID);
            
            CreateTable(
                "dbo.Towns",
                c => new
                    {
                        TownID = c.Int(nullable: false, identity: true),
                        TownCode = c.String(maxLength: 100),
                        TownLabel = c.String(),
                        RegionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TownID)
                .ForeignKey("dbo.Regions", t => t.RegionID)
                .Index(t => t.TownCode, unique: true, name: "TownCode")
                .Index(t => t.RegionID);
            
            CreateTable(
                "dbo.Regions",
                c => new
                    {
                        RegionID = c.Int(nullable: false, identity: true),
                        RegionCode = c.String(maxLength: 50),
                        RegionLabel = c.String(),
                        CountryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RegionID)
                .ForeignKey("dbo.Countries", t => t.CountryID)
                .Index(t => new { t.RegionCode, t.CountryID }, unique: true, name: "IX_RealPrimaryKey");
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        CountryID = c.Int(nullable: false, identity: true),
                        CountryCode = c.String(maxLength: 100),
                        CountryLabel = c.String(),
                    })
                .PrimaryKey(t => t.CountryID)
                .Index(t => t.CountryCode, unique: true, name: "CountryCode");
            
            CreateTable(
                "dbo.Devises",
                c => new
                    {
                        DeviseID = c.Int(nullable: false, identity: true),
                        DeviseCode = c.String(nullable: false, maxLength: 100),
                        DeviseLabel = c.String(),
                        DeviseDescription = c.String(),
                        DefaultDevise = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DeviseID)
                .Index(t => t.DeviseCode, unique: true, name: "DeviseCode");
            
            CreateTable(
                "dbo.Slices",
                c => new
                    {
                        SliceID = c.Int(nullable: false, identity: true),
                        SliceAmount = c.Double(nullable: false),
                        SliceDate = c.DateTime(nullable: false),
                        DeviseID = c.Int(nullable: false),
                        PaymentMethodID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SliceID)
                .ForeignKey("dbo.Devises", t => t.DeviseID)
                .ForeignKey("dbo.PaymentMethods", t => t.PaymentMethodID)
                .Index(t => t.DeviseID)
                .Index(t => t.PaymentMethodID);
            
            CreateTable(
                "dbo.PaymentMethods",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Code = c.String(maxLength: 100),
                        Description = c.String(),
                        BranchID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .Index(t => t.Code, unique: true)
                .Index(t => t.BranchID);
            
            CreateTable(
                "dbo.Deposits",
                c => new
                    {
                        DepositID = c.Int(nullable: false, identity: true),
                        Amount = c.Double(nullable: false),
                        DepositDate = c.DateTime(nullable: false),
                        PaymentMethodID = c.Int(nullable: false),
                        DeviseID = c.Int(nullable: false),
                        SavingAccountID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DepositID)
                .ForeignKey("dbo.Devises", t => t.DeviseID)
                .ForeignKey("dbo.PaymentMethods", t => t.PaymentMethodID)
                .ForeignKey("dbo.SavingAccounts", t => t.SavingAccountID)
                .Index(t => t.PaymentMethodID)
                .Index(t => t.DeviseID)
                .Index(t => t.SavingAccountID);
            
            CreateTable(
                "dbo.Sales",
                c => new
                    {
                        SaleID = c.Int(nullable: false, identity: true),
                        SaleDeliver = c.Boolean(nullable: false),
                        VatRate = c.Double(nullable: false),
                        RateReduction = c.Double(nullable: false),
                        RateDiscount = c.Double(nullable: false),
                        Transport = c.Double(nullable: false),
                        SaleDeliveryDate = c.DateTime(nullable: false),
                        SaleDate = c.DateTime(nullable: false),
                        SaleValidate = c.Boolean(nullable: false),
                        Representant = c.String(),
                        PaymentDelay = c.Int(nullable: false),
                        Guaranteed = c.Int(nullable: false),
                        Patient = c.String(),
                        DeviseID = c.Int(nullable: false),
                        IsPaid = c.Boolean(nullable: false),
                        SaleReceiptNumber = c.String(maxLength: 100),
                        BranchID = c.Int(nullable: false),
                        CustomerID = c.Int(nullable: false),
                        isReturn = c.Boolean(nullable: false),
                        StatutSale = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SaleID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .ForeignKey("dbo.Customers", t => t.CustomerID)
                .ForeignKey("dbo.Devises", t => t.DeviseID)
                .Index(t => t.DeviseID)
                .Index(t => t.SaleReceiptNumber, unique: true)
                .Index(t => t.BranchID)
                .Index(t => t.CustomerID);
            
            CreateTable(
                "dbo.Operations",
                c => new
                    {
                        OperationID = c.Int(nullable: false, identity: true),
                        OperationCode = c.String(maxLength: 50),
                        OperationLabel = c.String(),
                        OperationDescription = c.String(),
                        OperationTypeID = c.Int(nullable: false),
                        MacroOperationID = c.Int(nullable: false),
                        ReglementTypeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OperationID)
                .ForeignKey("dbo.MacroOperations", t => t.MacroOperationID)
                .ForeignKey("dbo.OperationTypes", t => t.OperationTypeID)
                .ForeignKey("dbo.ReglementTypes", t => t.ReglementTypeID)
                .Index(t => t.OperationCode, unique: true)
                .Index(t => t.OperationTypeID)
                .Index(t => t.MacroOperationID)
                .Index(t => t.ReglementTypeID);
            
            CreateTable(
                "dbo.MacroOperations",
                c => new
                    {
                        MacroOperationID = c.Int(nullable: false, identity: true),
                        MacroOperationCode = c.String(maxLength: 30),
                        MacroOperationLabel = c.String(),
                        MacroOperationDescription = c.String(),
                    })
                .PrimaryKey(t => t.MacroOperationID)
                .Index(t => t.MacroOperationCode, unique: true);
            
            CreateTable(
                "dbo.OperationTypes",
                c => new
                    {
                        operationTypeID = c.Int(nullable: false, identity: true),
                        operationTypeCode = c.String(maxLength: 30),
                        operationTypeLabel = c.String(),
                        operationTypeDescription = c.String(),
                    })
                .PrimaryKey(t => t.operationTypeID)
                .Index(t => t.operationTypeCode, unique: true);
            
            CreateTable(
                "dbo.Pieces",
                c => new
                    {
                        PieceID = c.Long(nullable: false, identity: true),
                        BranchID = c.Int(nullable: false),
                        DeviseID = c.Int(nullable: false),
                        OperationID = c.Int(nullable: false),
                        AccountID = c.Int(nullable: false),
                        DateOperation = c.DateTime(nullable: false),
                        Description = c.String(),
                        Reference = c.String(),
                        CodeTransaction = c.String(),
                        Debit = c.Double(nullable: false),
                        Credit = c.Double(nullable: false),
                        isAcctOperation = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PieceID)
                .ForeignKey("dbo.Accounts", t => t.AccountID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .ForeignKey("dbo.Devises", t => t.DeviseID)
                .ForeignKey("dbo.Operations", t => t.OperationID)
                .Index(t => t.BranchID)
                .Index(t => t.DeviseID)
                .Index(t => t.OperationID)
                .Index(t => t.AccountID);
            
            CreateTable(
                "dbo.ReglementTypes",
                c => new
                    {
                        ReglementTypeID = c.Int(nullable: false, identity: true),
                        ReglementTypeCode = c.String(maxLength: 30),
                        ReglementTypeLabel = c.String(),
                        ReglementTypeDescription = c.String(),
                    })
                .PrimaryKey(t => t.ReglementTypeID)
                .Index(t => t.ReglementTypeCode, unique: true);
            
            CreateTable(
                "dbo.Purchases",
                c => new
                    {
                        PurchaseID = c.Int(nullable: false, identity: true),
                        PurchaseDeliveryDate = c.DateTime(nullable: false),
                        PurchaseDate = c.DateTime(nullable: false),
                        VatRate = c.Double(nullable: false),
                        RateReduction = c.Double(nullable: false),
                        RateDiscount = c.Double(nullable: false),
                        Guaranteed = c.Int(nullable: false),
                        Transport = c.Double(nullable: false),
                        PaymentDelay = c.Int(nullable: false),
                        PurchaseValidate = c.Boolean(nullable: false),
                        isReturn = c.Boolean(nullable: false),
                        StatutPurchase = c.Int(nullable: false),
                        PurchaseReference = c.String(nullable: false, maxLength: 50),
                        PurchaseRegisterID = c.Int(nullable: false),
                        BranchID = c.Int(nullable: false),
                        DeviseID = c.Int(nullable: false),
                        PurchaseBringerID = c.Int(nullable: false),
                        SupplierID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PurchaseID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .ForeignKey("dbo.Devises", t => t.DeviseID)
                .ForeignKey("dbo.Users", t => t.PurchaseBringerID)
                .ForeignKey("dbo.Users", t => t.PurchaseRegisterID)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID)
                .Index(t => t.PurchaseReference, unique: true, name: "PurchaseReference")
                .Index(t => t.PurchaseRegisterID)
                .Index(t => t.BranchID)
                .Index(t => t.DeviseID)
                .Index(t => t.PurchaseBringerID)
                .Index(t => t.SupplierID);
            
            CreateTable(
                "dbo.BranchClosingDayTasks",
                c => new
                    {
                        BranchClosingDayTaskID = c.Int(nullable: false, identity: true),
                        Statut = c.Boolean(nullable: false),
                        ClosingDayTaskID = c.Int(nullable: false),
                        BranchID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BranchClosingDayTaskID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .ForeignKey("dbo.ClosingDayTasks", t => t.ClosingDayTaskID)
                .Index(t => t.ClosingDayTaskID)
                .Index(t => t.BranchID);
            
            CreateTable(
                "dbo.ClosingDayTasks",
                c => new
                    {
                        ClosingDayTaskID = c.Int(nullable: false, identity: true),
                        ClosingDayTaskCode = c.String(nullable: false, maxLength: 100),
                        ClosingDayTaskLabel = c.String(),
                        ClosingDayTaskDescription = c.String(),
                    })
                .PrimaryKey(t => t.ClosingDayTaskID)
                .Index(t => t.ClosingDayTaskCode, unique: true, name: "ClosingDayTaskCode");
            
            CreateTable(
                "dbo.BudgetConsumptions",
                c => new
                    {
                        BudgetConsumptionID = c.Int(nullable: false, identity: true),
                        BudgetAllocatedID = c.Int(nullable: false),
                        PaymentMethodID = c.Int(),
                        VoucherAmount = c.Double(nullable: false),
                        DateOperation = c.DateTime(nullable: false),
                        PaymentDate = c.DateTime(nullable: false),
                        Reference = c.String(),
                        isValidated = c.Boolean(nullable: false),
                        BeneficiaryName = c.String(),
                        Justification = c.String(),
                        DeviseID = c.Int(),
                    })
                .PrimaryKey(t => t.BudgetConsumptionID)
                .ForeignKey("dbo.BudgetAllocateds", t => t.BudgetAllocatedID)
                .ForeignKey("dbo.Devises", t => t.DeviseID)
                .ForeignKey("dbo.PaymentMethods", t => t.PaymentMethodID)
                .Index(t => t.BudgetAllocatedID)
                .Index(t => t.PaymentMethodID)
                .Index(t => t.DeviseID);
            
            CreateTable(
                "dbo.BudgetAllocateds",
                c => new
                    {
                        BudgetAllocatedID = c.Int(nullable: false, identity: true),
                        BranchID = c.Int(nullable: false),
                        FiscalYearID = c.Int(nullable: false),
                        BudgetLineID = c.Int(nullable: false),
                        AllocateAmount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.BudgetAllocatedID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .ForeignKey("dbo.BudgetLines", t => t.BudgetLineID)
                .ForeignKey("dbo.FiscalYears", t => t.FiscalYearID)
                .Index(t => t.BranchID)
                .Index(t => t.FiscalYearID)
                .Index(t => t.BudgetLineID);
            
            CreateTable(
                "dbo.BudgetAllocatedUpdates",
                c => new
                    {
                        BudgetAllocatedUpdateID = c.Int(nullable: false, identity: true),
                        BudgetAllocatedID = c.Int(nullable: false),
                        SensImputation = c.String(),
                        Justification = c.String(),
                        Amount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.BudgetAllocatedUpdateID)
                .ForeignKey("dbo.BudgetAllocateds", t => t.BudgetAllocatedID)
                .Index(t => t.BudgetAllocatedID);
            
            CreateTable(
                "dbo.BudgetLines",
                c => new
                    {
                        BudgetLineID = c.Int(nullable: false, identity: true),
                        BudgetCode = c.String(maxLength: 10),
                        BudgetLineLabel = c.String(),
                        BudgetType = c.String(maxLength: 10),
                        BudgetControl = c.Boolean(nullable: false),
                        AccountID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BudgetLineID)
                .ForeignKey("dbo.Accounts", t => t.AccountID)
                .Index(t => t.BudgetCode, unique: true, name: "BudgetCode")
                .Index(t => t.AccountID);
            
            CreateTable(
                "dbo.FiscalYears",
                c => new
                    {
                        FiscalYearID = c.Int(nullable: false, identity: true),
                        FiscalYearNumber = c.Int(nullable: false),
                        FiscalYearStatus = c.Boolean(nullable: false),
                        FiscalYearLabel = c.String(),
                        StartFrom = c.DateTime(nullable: false),
                        EndFrom = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.FiscalYearID)
                .Index(t => t.FiscalYearNumber, unique: true, name: "FiscalYearNumber")
                .Index(t => t.FiscalYearStatus, unique: true, name: "FiscalYearStatus");
            
            CreateTable(
                "dbo.ProductTransferts",
                c => new
                    {
                        ProductTransfertID = c.Int(nullable: false, identity: true),
                        IsReceived = c.Boolean(nullable: false),
                        ProductTransfertDate = c.DateTime(nullable: false),
                        ReceivedDate = c.DateTime(),
                        ProductTransfertReference = c.String(nullable: false, maxLength: 50),
                        DepartureBranchID = c.Int(nullable: false),
                        ArrivalBranchID = c.Int(nullable: false),
                        AskedByID = c.Int(nullable: false),
                        OrderedByID = c.Int(),
                        RegisteredByID = c.Int(nullable: false),
                        ReceivedByID = c.Int(),
                    })
                .PrimaryKey(t => t.ProductTransfertID)
                .ForeignKey("dbo.Branches", t => t.ArrivalBranchID)
                .ForeignKey("dbo.Users", t => t.AskedByID)
                .ForeignKey("dbo.Branches", t => t.DepartureBranchID)
                .ForeignKey("dbo.Users", t => t.OrderedByID)
                .ForeignKey("dbo.Users", t => t.ReceivedByID)
                .ForeignKey("dbo.Users", t => t.RegisteredByID)
                .Index(t => t.ProductTransfertReference, unique: true, name: "ProductTransfertReference")
                .Index(t => t.DepartureBranchID)
                .Index(t => t.ArrivalBranchID)
                .Index(t => t.AskedByID)
                .Index(t => t.OrderedByID)
                .Index(t => t.RegisteredByID)
                .Index(t => t.ReceivedByID);
            
            CreateTable(
                "dbo.ProductTransfertLines",
                c => new
                    {
                        ProductTransfertLineID = c.Int(nullable: false, identity: true),
                        ProductID = c.Int(nullable: false),
                        LineQuantity = c.Double(nullable: false),
                        DepartureLocalizationID = c.Int(nullable: false),
                        ArrivalLocalizationID = c.Int(nullable: false),
                        ProductTransfertID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductTransfertLineID)
                .ForeignKey("dbo.Localizations", t => t.ArrivalLocalizationID)
                .ForeignKey("dbo.Localizations", t => t.DepartureLocalizationID)
                .ForeignKey("dbo.Products", t => t.ProductID)
                .ForeignKey("dbo.ProductTransferts", t => t.ProductTransfertID)
                .Index(t => t.ProductID)
                .Index(t => t.DepartureLocalizationID)
                .Index(t => t.ArrivalLocalizationID)
                .Index(t => t.ProductTransfertID);
            
            CreateTable(
                "dbo.SupplierReturns",
                c => new
                    {
                        SupplierReturnID = c.Int(nullable: false, identity: true),
                        PurchaseID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SupplierReturnID)
                .ForeignKey("dbo.Purchases", t => t.PurchaseID)
                .Index(t => t.PurchaseID);
            
            CreateTable(
                "dbo.SupplierReturnLines",
                c => new
                    {
                        SupplierReturnLineID = c.Int(nullable: false, identity: true),
                        SupplierReturnID = c.Int(nullable: false),
                        Transport = c.Double(),
                        SupplierReturnCauses = c.String(),
                        PurchaseLineID = c.Int(nullable: false),
                        LineQuantity = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.SupplierReturnLineID)
                .ForeignKey("dbo.PurchaseLines", t => t.PurchaseLineID)
                .ForeignKey("dbo.SupplierReturns", t => t.SupplierReturnID)
                .Index(t => t.SupplierReturnID)
                .Index(t => t.PurchaseLineID);
            
            CreateTable(
                "dbo.CustomerReturns",
                c => new
                    {
                        CustomerReturnID = c.Int(nullable: false, identity: true),
                        SaleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerReturnID)
                .ForeignKey("dbo.Sales", t => t.SaleID)
                .Index(t => t.SaleID, unique: true, name: "SaleID_IX");
            
            CreateTable(
                "dbo.CustomerReturnLines",
                c => new
                    {
                        CustomerReturnLineID = c.Int(nullable: false, identity: true),
                        CustomerReturnDate = c.DateTime(nullable: false),
                        Transport = c.Double(),
                        CustomerReturnCauses = c.String(),
                        SaleLineID = c.Int(nullable: false),
                        CustomerReturnID = c.Int(nullable: false),
                        LineQuantity = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerReturnLineID)
                .ForeignKey("dbo.CustomerReturns", t => t.CustomerReturnID)
                .ForeignKey("dbo.SaleLines", t => t.SaleLineID)
                .Index(t => t.SaleLineID)
                .Index(t => t.CustomerReturnID);
            
            CreateTable(
                "dbo.TillAdjusts",
                c => new
                    {
                        TillAdjustID = c.Int(nullable: false, identity: true),
                        TillAdjustDate = c.DateTime(nullable: false),
                        ComputerPrice = c.Double(nullable: false),
                        PhysicalPrice = c.Double(nullable: false),
                        Justification = c.String(),
                        TillID = c.Int(nullable: false),
                        DeviseID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TillAdjustID)
                .ForeignKey("dbo.Devises", t => t.DeviseID)
                .ForeignKey("dbo.Tills", t => t.TillID)
                .Index(t => t.TillID)
                .Index(t => t.DeviseID);
            
            CreateTable(
                "dbo.ClassAccounts",
                c => new
                    {
                        ClassAccountID = c.Int(nullable: false, identity: true),
                        ClassAccountNumber = c.Int(nullable: false),
                        ClassAccountCode = c.String(nullable: false, maxLength: 100),
                        ClassAccountLabel = c.String(),
                    })
                .PrimaryKey(t => t.ClassAccountID)
                .Index(t => t.ClassAccountNumber, unique: true, name: "ClassAccountNumber")
                .Index(t => t.ClassAccountCode, unique: true, name: "ClassAccountCode");
            
            CreateTable(
                "dbo.BusinessDays",
                c => new
                    {
                        BusinessDayID = c.Int(nullable: false, identity: true),
                        BDCode = c.String(nullable: false, maxLength: 100),
                        BDLabel = c.String(),
                        BDDescription = c.String(),
                        BDDateOperation = c.DateTime(nullable: false),
                        BDStatut = c.Boolean(nullable: false),
                        ClosingDayStarted = c.Boolean(nullable: false),
                        BranchID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BusinessDayID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .Index(t => t.BDCode, unique: true, name: "BDCode")
                .Index(t => t.BranchID);
            
            CreateTable(
                "dbo.EmployeeStockHistorics",
                c => new
                    {
                        EmployeeStockHistoricID = c.Int(nullable: false, identity: true),
                        AssigningDate = c.DateTime(nullable: false),
                        RemovingDate = c.DateTime(nullable: false),
                        WareHouseManID = c.Int(nullable: false),
                        WareHouseID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeStockHistoricID)
                .ForeignKey("dbo.Localizations", t => t.WareHouseID)
                .ForeignKey("dbo.Users", t => t.WareHouseManID)
                .Index(t => t.WareHouseManID)
                .Index(t => t.WareHouseID);
            
            CreateTable(
                "dbo.EmployeeStocks",
                c => new
                    {
                        EmployeeStockID = c.Int(nullable: false, identity: true),
                        AssigningDate = c.DateTime(nullable: false),
                        IsPrincipalWareHouseMan = c.Boolean(nullable: false),
                        WareHouseManID = c.Int(nullable: false),
                        WareHouseID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeStockID)
                .ForeignKey("dbo.Localizations", t => t.WareHouseID)
                .ForeignKey("dbo.Users", t => t.WareHouseManID)
                .Index(t => t.WareHouseManID)
                .Index(t => t.WareHouseID);
            
            CreateTable(
                "dbo.InventoryDirectories",
                c => new
                    {
                        InventoryDirectoryID = c.Int(nullable: false, identity: true),
                        InventoryDirectoryReference = c.String(),
                        InventoryDirectoryDescription = c.String(),
                        InventoryDirectoryCreationDate = c.DateTime(nullable: false),
                        InventoryDirectoryDate = c.DateTime(nullable: false),
                        InventoryDirectoryStatut = c.Int(nullable: false),
                        BranchID = c.Int(nullable: false),
                        RegisteredByID = c.Int(),
                    })
                .PrimaryKey(t => t.InventoryDirectoryID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .ForeignKey("dbo.Users", t => t.RegisteredByID)
                .Index(t => t.BranchID)
                .Index(t => t.RegisteredByID);
            
            CreateTable(
                "dbo.InventoryDirectoryLines",
                c => new
                    {
                        InventoryDirectoryLineID = c.Int(nullable: false, identity: true),
                        inventoryReason = c.String(),
                        OldStockQuantity = c.Double(nullable: false),
                        NewStockQuantity = c.Double(),
                        OldSafetyStockQuantity = c.Double(nullable: false),
                        NewSafetyStockQuantity = c.Double(),
                        ProductID = c.Int(nullable: false),
                        LocalizationID = c.Int(nullable: false),
                        AutorizedByID = c.Int(),
                        CountByID = c.Int(),
                        RegisteredByID = c.Int(),
                        InventoryDirectoryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InventoryDirectoryLineID)
                .ForeignKey("dbo.Users", t => t.AutorizedByID)
                .ForeignKey("dbo.Users", t => t.CountByID)
                .ForeignKey("dbo.InventoryDirectories", t => t.InventoryDirectoryID)
                .ForeignKey("dbo.Localizations", t => t.LocalizationID)
                .ForeignKey("dbo.Products", t => t.ProductID)
                .ForeignKey("dbo.Users", t => t.RegisteredByID)
                .Index(t => t.ProductID)
                .Index(t => t.LocalizationID)
                .Index(t => t.AutorizedByID)
                .Index(t => t.CountByID)
                .Index(t => t.RegisteredByID)
                .Index(t => t.InventoryDirectoryID);
            
            CreateTable(
                "dbo.InventoryHistorics",
                c => new
                    {
                        InventoryHistoricID = c.Int(nullable: false, identity: true),
                        InventoryDate = c.DateTime(nullable: false),
                        inventoryReason = c.String(),
                        OldStockQuantity = c.Double(nullable: false),
                        NewStockQuantity = c.Double(nullable: false),
                        OldStockUnitPrice = c.Double(nullable: false),
                        NEwStockUnitPrice = c.Double(nullable: false),
                        OldSafetyStockQuantity = c.Double(nullable: false),
                        NewSafetyStockQuantity = c.Double(nullable: false),
                        ProductID = c.Int(nullable: false),
                        LocalizationID = c.Int(nullable: false),
                        AutorizedByID = c.Int(nullable: false),
                        CountByID = c.Int(),
                        RegisteredByID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InventoryHistoricID)
                .ForeignKey("dbo.Users", t => t.AutorizedByID)
                .ForeignKey("dbo.Users", t => t.CountByID)
                .ForeignKey("dbo.Localizations", t => t.LocalizationID)
                .ForeignKey("dbo.Products", t => t.ProductID)
                .ForeignKey("dbo.Users", t => t.RegisteredByID)
                .Index(t => t.ProductID)
                .Index(t => t.LocalizationID)
                .Index(t => t.AutorizedByID)
                .Index(t => t.CountByID)
                .Index(t => t.RegisteredByID);
            
            CreateTable(
                "dbo.LensNumberRangePrices",
                c => new
                    {
                        LensNumberRangePriceID = c.Int(nullable: false, identity: true),
                        LensCategoryID = c.Int(nullable: false),
                        SphericalValueRangeID = c.Int(),
                        CylindricalValueRangeID = c.Int(),
                        AdditionValueRangeID = c.Int(),
                        Price = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.LensNumberRangePriceID)
                .ForeignKey("dbo.LensNumberRanges", t => t.AdditionValueRangeID)
                .ForeignKey("dbo.LensNumberRanges", t => t.CylindricalValueRangeID)
                .ForeignKey("dbo.LensCategories", t => t.LensCategoryID)
                .ForeignKey("dbo.LensNumberRanges", t => t.SphericalValueRangeID)
                .Index(t => t.LensCategoryID)
                .Index(t => t.SphericalValueRangeID)
                .Index(t => t.CylindricalValueRangeID)
                .Index(t => t.AdditionValueRangeID);
            
            CreateTable(
                "dbo.LensNumberRanges",
                c => new
                    {
                        LensNumberRangeID = c.Int(nullable: false, identity: true),
                        Minimum = c.String(maxLength: 10),
                        Maximum = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.LensNumberRangeID)
                .Index(t => new { t.Minimum, t.Maximum }, unique: true, name: "IX_RealPrimaryKey");
            
            CreateTable(
                "dbo.SupplierOrders",
                c => new
                    {
                        SupplierOrderID = c.Int(nullable: false, identity: true),
                        SupplierOrderReference = c.String(maxLength: 50),
                        SupplierOrderDate = c.DateTime(nullable: false),
                        VatRate = c.Double(nullable: false),
                        RateReduction = c.Double(nullable: false),
                        RateDiscount = c.Double(nullable: false),
                        Transport = c.Double(nullable: false),
                        IsDelivered = c.Boolean(nullable: false),
                        SupplierID = c.Int(nullable: false),
                        DeviseID = c.Int(nullable: false),
                        BranchID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SupplierOrderID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .ForeignKey("dbo.Devises", t => t.DeviseID)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID)
                .Index(t => t.SupplierOrderReference, unique: true)
                .Index(t => t.SupplierID)
                .Index(t => t.DeviseID)
                .Index(t => t.BranchID);
            
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
                .PrimaryKey(t => t.RptCashOpHistID);
            
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
                "dbo.RptInventories",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BranchName = c.String(),
                        BranchAdress = c.String(),
                        BranchTel = c.String(),
                        CompanyName = c.String(),
                        CompanyAdress = c.String(),
                        CompanyTel = c.String(),
                        CompanyCNI = c.String(),
                        CompanyLogo = c.Binary(),
                        ProductLabel = c.String(),
                        ProductQty = c.Int(nullable: false),
                        ProductUnitPrice = c.Double(nullable: false),
                        Localization = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
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
                    })
                .PrimaryKey(t => t.RptPrintStmtID);
            
            CreateTable(
                "dbo.RptReceipts",
                c => new
                    {
                        RptReceiptID = c.Int(nullable: false, identity: true),
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
                        SaleDate = c.DateTime(nullable: false),
                        ProductLabel = c.String(),
                        ProductRef = c.String(),
                        LineUnitPrice = c.Double(nullable: false),
                        LineQuantity = c.Double(nullable: false),
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
                    })
                .PrimaryKey(t => t.RptReceiptID);
            
            CreateTable(
                "dbo.SneakDays",
                c => new
                    {
                        SneakDayID = c.Int(nullable: false, identity: true),
                        SneakDayDate = c.DateTime(nullable: false),
                        SneakDayDatabaseTables = c.String(),
                        SneakDayDescription = c.String(),
                        SneakDayOperationType = c.String(),
                        SneakDayOldValue = c.String(),
                        SneakDayNewValue = c.String(),
                        SneakDayUserNames = c.String(),
                        SneakDayHost = c.String(),
                        SneakDayHostAdress = c.String(),
                        SneakDayBranchNames = c.String(),
                    })
                .PrimaryKey(t => t.SneakDayID);
            
            CreateTable(
                "dbo.Sneaks",
                c => new
                    {
                        SneakID = c.Int(nullable: false),
                        SneakDate = c.DateTime(nullable: false),
                        SneakDatabaseTables = c.String(),
                        SneakDescription = c.String(),
                        SneakOperationType = c.String(),
                        SneakOldValue = c.String(),
                        SneakNewValue = c.String(),
                        SneakUserNames = c.String(),
                        SneakHost = c.String(),
                        SneakHostAdress = c.String(),
                        SneakBranchNames = c.String(),
                    })
                .PrimaryKey(t => t.SneakID);
            
            CreateTable(
                "dbo.TillDays",
                c => new
                    {
                        TillDayID = c.Int(nullable: false, identity: true),
                        TillDayDate = c.DateTime(nullable: false),
                        IsOpen = c.Boolean(nullable: false),
                        TillDayOpenPrice = c.Double(nullable: false),
                        TillDayClosingPrice = c.Double(nullable: false),
                        TillID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TillDayID)
                .ForeignKey("dbo.Tills", t => t.TillID)
                .Index(t => t.TillID);
            
            CreateTable(
                "dbo.TransactNumbers",
                c => new
                    {
                        TransactNumberID = c.Int(nullable: false, identity: true),
                        TransactNumberCode = c.String(maxLength: 16),
                        MaxCounter = c.Int(nullable: false),
                        Counter = c.Int(nullable: false),
                        DateOperation = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TransactNumberID)
                .Index(t => t.TransactNumberCode, unique: true);
            
            CreateTable(
                "dbo.UserTills",
                c => new
                    {
                        UserTillID = c.Int(nullable: false, identity: true),
                        UserTillDateAssignment = c.DateTime(nullable: false),
                        HasAccess = c.Boolean(nullable: false),
                        TillID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                        UserTillDisAssignDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserTillID)
                .ForeignKey("dbo.Tills", t => t.TillID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .Index(t => t.TillID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.AccountingSpecificTask",
                c => new
                    {
                        AccountingTaskID = c.Int(nullable: false),
                        AccountID = c.Int(),
                        VatAccountID = c.Int(),
                        DiscountAccountID = c.Int(),
                        TransportAccountID = c.Int(),
                    })
                .PrimaryKey(t => t.AccountingTaskID)
                .ForeignKey("dbo.Accounts", t => t.AccountID)
                .ForeignKey("dbo.Accounts", t => t.VatAccountID)
                .ForeignKey("dbo.Accounts", t => t.DiscountAccountID)
                .ForeignKey("dbo.Accounts", t => t.TransportAccountID)
                .ForeignKey("dbo.AccountingTask", t => t.AccountingTaskID)
                .Index(t => t.AccountingTaskID)
                .Index(t => t.AccountID)
                .Index(t => t.VatAccountID)
                .Index(t => t.DiscountAccountID)
                .Index(t => t.TransportAccountID);
            
            CreateTable(
                "dbo.BankPurchases",
                c => new
                    {
                        PurchaseID = c.Int(nullable: false),
                        BankID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PurchaseID)
                .ForeignKey("dbo.Purchases", t => t.PurchaseID)
                .ForeignKey("dbo.Banks", t => t.BankID)
                .Index(t => t.PurchaseID)
                .Index(t => t.BankID);
            
            CreateTable(
                "dbo.BankSales",
                c => new
                    {
                        SaleID = c.Int(nullable: false),
                        BankID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SaleID)
                .ForeignKey("dbo.Sales", t => t.SaleID)
                .ForeignKey("dbo.Banks", t => t.BankID)
                .Index(t => t.SaleID)
                .Index(t => t.BankID);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        GlobalPersonID = c.Int(nullable: false),
                        CompanyCapital = c.Int(nullable: false),
                        CompanySigle = c.String(maxLength: 50),
                        CompanyTradeRegister = c.String(maxLength: 50),
                        CompanySlogan = c.String(),
                        CompanyIsDeletable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.GlobalPersonID)
                .ForeignKey("dbo.GlobalPeople", t => t.GlobalPersonID)
                .Index(t => t.GlobalPersonID)
                .Index(t => t.CompanySigle, unique: true, name: "CompanySigle")
                .Index(t => t.CompanyTradeRegister, unique: true, name: "CompanyTradeRegister");
            
            CreateTable(
                "dbo.CustomerOrderLines",
                c => new
                    {
                        LineID = c.Int(nullable: false),
                        CustomerOrderID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LineID)
                .ForeignKey("dbo.Lines", t => t.LineID)
                .ForeignKey("dbo.CustomerOrders", t => t.CustomerOrderID)
                .Index(t => t.LineID)
                .Index(t => t.CustomerOrderID);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        GlobalPersonID = c.Int(nullable: false),
                        Adress_AdressID = c.Int(),
                        IsConnected = c.Boolean(nullable: false),
                        SexID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GlobalPersonID)
                .ForeignKey("dbo.GlobalPeople", t => t.GlobalPersonID)
                .ForeignKey("dbo.Adresses", t => t.Adress_AdressID)
                .ForeignKey("dbo.Sexes", t => t.SexID)
                .Index(t => t.GlobalPersonID)
                .Index(t => t.Adress_AdressID)
                .Index(t => t.SexID);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        GlobalPersonID = c.Int(nullable: false),
                        AccountID = c.Int(nullable: false),
                        CustomerNumber = c.String(maxLength: 250),
                        CompanyCapital = c.Int(nullable: false),
                        CompanySigle = c.String(),
                        CompanyTradeRegister = c.String(),
                        CompanySlogan = c.String(),
                        CompanyIsDeletable = c.Boolean(nullable: false),
                        IsCashCustomer = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GlobalPersonID)
                .ForeignKey("dbo.People", t => t.GlobalPersonID)
                .ForeignKey("dbo.Accounts", t => t.AccountID)
                .Index(t => t.GlobalPersonID)
                .Index(t => t.AccountID)
                .Index(t => t.CustomerNumber, unique: true, name: "CustomerNumber");
            
            CreateTable(
                "dbo.CustomerSlices",
                c => new
                    {
                        SliceID = c.Int(nullable: false),
                        SaleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SliceID)
                .ForeignKey("dbo.Slices", t => t.SliceID)
                .ForeignKey("dbo.Sales", t => t.SaleID)
                .Index(t => t.SliceID)
                .Index(t => t.SaleID);
            
            CreateTable(
                "dbo.GenericProducts",
                c => new
                    {
                        ProductID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductID)
                .ForeignKey("dbo.Products", t => t.ProductID)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.LensCategories",
                c => new
                    {
                        CategoryID = c.Int(nullable: false),
                        BifocalCode = c.String(maxLength: 10),
                        IsProgressive = c.Boolean(nullable: false),
                        LensMaterialID = c.Int(nullable: false),
                        CollectifAccountID = c.Int(nullable: false),
                        LensCoatingID = c.Int(nullable: false),
                        LensColourID = c.Int(nullable: false),
                        SupplyingName = c.String(),
                    })
                .PrimaryKey(t => t.CategoryID)
                .ForeignKey("dbo.Categories", t => t.CategoryID)
                .ForeignKey("dbo.LensMaterials", t => t.LensMaterialID)
                .ForeignKey("dbo.CollectifAccounts", t => t.CollectifAccountID)
                .ForeignKey("dbo.LensCoatings", t => t.LensCoatingID)
                .ForeignKey("dbo.LensColours", t => t.LensColourID)
                .Index(t => t.CategoryID)
                .Index(t => t.LensMaterialID)
                .Index(t => t.CollectifAccountID)
                .Index(t => t.LensCoatingID)
                .Index(t => t.LensColourID);
            
            CreateTable(
                "dbo.Lenses",
                c => new
                    {
                        ProductID = c.Int(nullable: false),
                        LensCategoryID = c.Int(nullable: false),
                        LensNumberID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductID)
                .ForeignKey("dbo.Products", t => t.ProductID)
                .ForeignKey("dbo.LensCategories", t => t.LensCategoryID)
                .ForeignKey("dbo.LensNumbers", t => t.LensNumberID)
                .Index(t => t.ProductID)
                .Index(t => t.LensCategoryID)
                .Index(t => t.LensNumberID);
            
            CreateTable(
                "dbo.OrderLenses",
                c => new
                    {
                        ProductID = c.Int(nullable: false),
                        EyeSide = c.Int(nullable: false),
                        Addition = c.String(),
                        Axis = c.String(),
                        Index = c.String(),
                        LensCategoryID = c.Int(nullable: false),
                        LensNumberID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductID)
                .ForeignKey("dbo.Products", t => t.ProductID)
                .ForeignKey("dbo.LensCategories", t => t.LensCategoryID)
                .ForeignKey("dbo.LensNumbers", t => t.LensNumberID)
                .Index(t => t.ProductID)
                .Index(t => t.LensCategoryID)
                .Index(t => t.LensNumberID);
            
            CreateTable(
                "dbo.PurchaseLines",
                c => new
                    {
                        LineID = c.Int(nullable: false),
                        PurchaseID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LineID)
                .ForeignKey("dbo.Lines", t => t.LineID)
                .ForeignKey("dbo.Purchases", t => t.PurchaseID)
                .Index(t => t.LineID)
                .Index(t => t.PurchaseID);
            
            CreateTable(
                "dbo.SaleLines",
                c => new
                    {
                        LineID = c.Int(nullable: false),
                        SaleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LineID)
                .ForeignKey("dbo.Lines", t => t.LineID)
                .ForeignKey("dbo.Sales", t => t.SaleID)
                .Index(t => t.LineID)
                .Index(t => t.SaleID);
            
            CreateTable(
                "dbo.SavingAccounts",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        CustomerID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.PaymentMethods", t => t.ID)
                .ForeignKey("dbo.Customers", t => t.CustomerID)
                .Index(t => t.ID)
                .Index(t => t.CustomerID);
            
            CreateTable(
                "dbo.SavingAccountSales",
                c => new
                    {
                        SaleID = c.Int(nullable: false),
                        SavingAccountID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SaleID)
                .ForeignKey("dbo.Sales", t => t.SaleID)
                .ForeignKey("dbo.SavingAccounts", t => t.SavingAccountID)
                .Index(t => t.SaleID)
                .Index(t => t.SavingAccountID);
            
            CreateTable(
                "dbo.SpecialOrders",
                c => new
                    {
                        CustomerOrderID = c.Int(nullable: false),
                        OrderStatut = c.Int(nullable: false),
                        ValidatedDate = c.DateTime(),
                        ReceivedDate = c.DateTime(),
                        DeliveredDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.CustomerOrderID)
                .ForeignKey("dbo.CustomerOrders", t => t.CustomerOrderID)
                .Index(t => t.CustomerOrderID);
            
            CreateTable(
                "dbo.SpecialOrderSlices",
                c => new
                    {
                        SliceID = c.Int(nullable: false),
                        SpecialOrderID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SliceID)
                .ForeignKey("dbo.Slices", t => t.SliceID)
                .ForeignKey("dbo.SpecialOrders", t => t.SpecialOrderID)
                .Index(t => t.SliceID)
                .Index(t => t.SpecialOrderID);
            
            CreateTable(
                "dbo.SupplierOrderLines",
                c => new
                    {
                        LineID = c.Int(nullable: false),
                        SupplierOrderID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LineID)
                .ForeignKey("dbo.Lines", t => t.LineID)
                .ForeignKey("dbo.SupplierOrders", t => t.SupplierOrderID)
                .Index(t => t.LineID)
                .Index(t => t.SupplierOrderID);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        GlobalPersonID = c.Int(nullable: false),
                        AccountID = c.Int(nullable: false),
                        SupplierNumber = c.String(maxLength: 250),
                        CompanyCapital = c.Int(nullable: false),
                        CompanySigle = c.String(),
                        CompanyTradeRegister = c.String(),
                        CompanySlogan = c.String(),
                        CompanyIsDeletable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.GlobalPersonID)
                .ForeignKey("dbo.People", t => t.GlobalPersonID)
                .ForeignKey("dbo.Accounts", t => t.AccountID)
                .Index(t => t.GlobalPersonID)
                .Index(t => t.AccountID)
                .Index(t => t.SupplierNumber, unique: true, name: "SupplierNumber");
            
            CreateTable(
                "dbo.SupplierSlices",
                c => new
                    {
                        SliceID = c.Int(nullable: false),
                        PurchaseID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SliceID)
                .ForeignKey("dbo.Slices", t => t.SliceID)
                .ForeignKey("dbo.Purchases", t => t.PurchaseID)
                .Index(t => t.SliceID)
                .Index(t => t.PurchaseID);
            
            CreateTable(
                "dbo.TillPurchases",
                c => new
                    {
                        PurchaseID = c.Int(nullable: false),
                        TillID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PurchaseID)
                .ForeignKey("dbo.Purchases", t => t.PurchaseID)
                .ForeignKey("dbo.Tills", t => t.TillID)
                .Index(t => t.PurchaseID)
                .Index(t => t.TillID);
            
            CreateTable(
                "dbo.TillSales",
                c => new
                    {
                        SaleID = c.Int(nullable: false),
                        TillID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SaleID)
                .ForeignKey("dbo.Sales", t => t.SaleID)
                .ForeignKey("dbo.Tills", t => t.TillID)
                .Index(t => t.SaleID)
                .Index(t => t.TillID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        GlobalPersonID = c.Int(nullable: false),
                        Code = c.String(nullable: false, maxLength: 100),
                        UserLogin = c.String(maxLength: 100),
                        UserPassword = c.String(nullable: false),
                        UserAccountState = c.Boolean(nullable: false),
                        UserAccessLevel = c.Int(nullable: false),
                        ProfileID = c.Int(nullable: false),
                        UserConfigurationID = c.Int(),
                        JobID = c.Int(),
                    })
                .PrimaryKey(t => t.GlobalPersonID)
                .ForeignKey("dbo.People", t => t.GlobalPersonID)
                .ForeignKey("dbo.Profiles", t => t.ProfileID)
                .ForeignKey("dbo.UserConfigurations", t => t.UserConfigurationID)
                .ForeignKey("dbo.Jobs", t => t.JobID)
                .Index(t => t.GlobalPersonID)
                .Index(t => t.Code, unique: true, name: "Code")
                .Index(t => t.UserLogin, unique: true, name: "UserLogin")
                .Index(t => t.ProfileID)
                .Index(t => t.UserConfigurationID)
                .Index(t => t.JobID);
            
            CreateTable(
                "dbo.Banks",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        AccountID = c.Int(nullable: false),
                        BankAgrement = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.PaymentMethods", t => t.ID)
                .ForeignKey("dbo.Accounts", t => t.AccountID)
                .Index(t => t.ID)
                .Index(t => t.AccountID);
            
            CreateTable(
                "dbo.Tills",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        AccountID = c.Int(nullable: false),
                        TillMaxMontant = c.Int(nullable: false),
                        TillComment = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.PaymentMethods", t => t.ID)
                .ForeignKey("dbo.Accounts", t => t.AccountID)
                .Index(t => t.ID)
                .Index(t => t.AccountID);
            
            CreateTable(
                "dbo.ManualAccountOperations",
                c => new
                    {
                        AccountOperationID = c.Long(nullable: false),
                        PieceID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.AccountOperationID)
                .ForeignKey("dbo.AccountOperations", t => t.AccountOperationID)
                .ForeignKey("dbo.Pieces", t => t.PieceID)
                .Index(t => t.AccountOperationID)
                .Index(t => t.PieceID);
            
            CreateTable(
                "dbo.SaleAccountOperations",
                c => new
                    {
                        AccountOperationID = c.Long(nullable: false),
                        SaleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountOperationID)
                .ForeignKey("dbo.AccountOperations", t => t.AccountOperationID)
                .ForeignKey("dbo.Sales", t => t.SaleID)
                .Index(t => t.AccountOperationID)
                .Index(t => t.SaleID);
            
            CreateTable(
                "dbo.PurchaseAccountOperations",
                c => new
                    {
                        AccountOperationID = c.Long(nullable: false),
                        PurchaseID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountOperationID)
                .ForeignKey("dbo.AccountOperations", t => t.AccountOperationID)
                .ForeignKey("dbo.Purchases", t => t.PurchaseID)
                .Index(t => t.AccountOperationID)
                .Index(t => t.PurchaseID);
            
            CreateTable(
                "dbo.ProductTransferAccountOperations",
                c => new
                    {
                        AccountOperationID = c.Long(nullable: false),
                        ProductTransfertID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountOperationID)
                .ForeignKey("dbo.AccountOperations", t => t.AccountOperationID)
                .ForeignKey("dbo.ProductTransferts", t => t.ProductTransfertID)
                .Index(t => t.AccountOperationID)
                .Index(t => t.ProductTransfertID);
            
            CreateTable(
                "dbo.SaleReturnAccountOperations",
                c => new
                    {
                        AccountOperationID = c.Long(nullable: false),
                        CustomerReturnID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountOperationID)
                .ForeignKey("dbo.AccountOperations", t => t.AccountOperationID)
                .ForeignKey("dbo.CustomerReturns", t => t.CustomerReturnID)
                .Index(t => t.AccountOperationID)
                .Index(t => t.CustomerReturnID);
            
            CreateTable(
                "dbo.PurchaseReturnAccountOperations",
                c => new
                    {
                        AccountOperationID = c.Long(nullable: false),
                        SupplierReturnID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountOperationID)
                .ForeignKey("dbo.AccountOperations", t => t.AccountOperationID)
                .ForeignKey("dbo.SupplierReturns", t => t.SupplierReturnID)
                .Index(t => t.AccountOperationID)
                .Index(t => t.SupplierReturnID);
            
            CreateTable(
                "dbo.ProductLocalizationAccountOperations",
                c => new
                    {
                        AccountOperationID = c.Long(nullable: false),
                        ProductLocalizationID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountOperationID)
                .ForeignKey("dbo.AccountOperations", t => t.AccountOperationID)
                .ForeignKey("dbo.ProductLocalizations", t => t.ProductLocalizationID)
                .Index(t => t.AccountOperationID)
                .Index(t => t.ProductLocalizationID);
            
            CreateTable(
                "dbo.BudgetConsumptionAccountOperations",
                c => new
                    {
                        AccountOperationID = c.Long(nullable: false),
                        BudgetConsumptionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountOperationID)
                .ForeignKey("dbo.AccountOperations", t => t.AccountOperationID)
                .ForeignKey("dbo.BudgetConsumptions", t => t.BudgetConsumptionID)
                .Index(t => t.AccountOperationID)
                .Index(t => t.BudgetConsumptionID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BudgetConsumptionAccountOperations", "BudgetConsumptionID", "dbo.BudgetConsumptions");
            DropForeignKey("dbo.BudgetConsumptionAccountOperations", "AccountOperationID", "dbo.AccountOperations");
            DropForeignKey("dbo.ProductLocalizationAccountOperations", "ProductLocalizationID", "dbo.ProductLocalizations");
            DropForeignKey("dbo.ProductLocalizationAccountOperations", "AccountOperationID", "dbo.AccountOperations");
            DropForeignKey("dbo.PurchaseReturnAccountOperations", "SupplierReturnID", "dbo.SupplierReturns");
            DropForeignKey("dbo.PurchaseReturnAccountOperations", "AccountOperationID", "dbo.AccountOperations");
            DropForeignKey("dbo.SaleReturnAccountOperations", "CustomerReturnID", "dbo.CustomerReturns");
            DropForeignKey("dbo.SaleReturnAccountOperations", "AccountOperationID", "dbo.AccountOperations");
            DropForeignKey("dbo.ProductTransferAccountOperations", "ProductTransfertID", "dbo.ProductTransferts");
            DropForeignKey("dbo.ProductTransferAccountOperations", "AccountOperationID", "dbo.AccountOperations");
            DropForeignKey("dbo.PurchaseAccountOperations", "PurchaseID", "dbo.Purchases");
            DropForeignKey("dbo.PurchaseAccountOperations", "AccountOperationID", "dbo.AccountOperations");
            DropForeignKey("dbo.SaleAccountOperations", "SaleID", "dbo.Sales");
            DropForeignKey("dbo.SaleAccountOperations", "AccountOperationID", "dbo.AccountOperations");
            DropForeignKey("dbo.ManualAccountOperations", "PieceID", "dbo.Pieces");
            DropForeignKey("dbo.ManualAccountOperations", "AccountOperationID", "dbo.AccountOperations");
            DropForeignKey("dbo.Tills", "AccountID", "dbo.Accounts");
            DropForeignKey("dbo.Tills", "ID", "dbo.PaymentMethods");
            DropForeignKey("dbo.Banks", "AccountID", "dbo.Accounts");
            DropForeignKey("dbo.Banks", "ID", "dbo.PaymentMethods");
            DropForeignKey("dbo.Users", "JobID", "dbo.Jobs");
            DropForeignKey("dbo.Users", "UserConfigurationID", "dbo.UserConfigurations");
            DropForeignKey("dbo.Users", "ProfileID", "dbo.Profiles");
            DropForeignKey("dbo.Users", "GlobalPersonID", "dbo.People");
            DropForeignKey("dbo.TillSales", "TillID", "dbo.Tills");
            DropForeignKey("dbo.TillSales", "SaleID", "dbo.Sales");
            DropForeignKey("dbo.TillPurchases", "TillID", "dbo.Tills");
            DropForeignKey("dbo.TillPurchases", "PurchaseID", "dbo.Purchases");
            DropForeignKey("dbo.SupplierSlices", "PurchaseID", "dbo.Purchases");
            DropForeignKey("dbo.SupplierSlices", "SliceID", "dbo.Slices");
            DropForeignKey("dbo.Suppliers", "AccountID", "dbo.Accounts");
            DropForeignKey("dbo.Suppliers", "GlobalPersonID", "dbo.People");
            DropForeignKey("dbo.SupplierOrderLines", "SupplierOrderID", "dbo.SupplierOrders");
            DropForeignKey("dbo.SupplierOrderLines", "LineID", "dbo.Lines");
            DropForeignKey("dbo.SpecialOrderSlices", "SpecialOrderID", "dbo.SpecialOrders");
            DropForeignKey("dbo.SpecialOrderSlices", "SliceID", "dbo.Slices");
            DropForeignKey("dbo.SpecialOrders", "CustomerOrderID", "dbo.CustomerOrders");
            DropForeignKey("dbo.SavingAccountSales", "SavingAccountID", "dbo.SavingAccounts");
            DropForeignKey("dbo.SavingAccountSales", "SaleID", "dbo.Sales");
            DropForeignKey("dbo.SavingAccounts", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.SavingAccounts", "ID", "dbo.PaymentMethods");
            DropForeignKey("dbo.SaleLines", "SaleID", "dbo.Sales");
            DropForeignKey("dbo.SaleLines", "LineID", "dbo.Lines");
            DropForeignKey("dbo.PurchaseLines", "PurchaseID", "dbo.Purchases");
            DropForeignKey("dbo.PurchaseLines", "LineID", "dbo.Lines");
            DropForeignKey("dbo.OrderLenses", "LensNumberID", "dbo.LensNumbers");
            DropForeignKey("dbo.OrderLenses", "LensCategoryID", "dbo.LensCategories");
            DropForeignKey("dbo.OrderLenses", "ProductID", "dbo.Products");
            DropForeignKey("dbo.Lenses", "LensNumberID", "dbo.LensNumbers");
            DropForeignKey("dbo.Lenses", "LensCategoryID", "dbo.LensCategories");
            DropForeignKey("dbo.Lenses", "ProductID", "dbo.Products");
            DropForeignKey("dbo.LensCategories", "LensColourID", "dbo.LensColours");
            DropForeignKey("dbo.LensCategories", "LensCoatingID", "dbo.LensCoatings");
            DropForeignKey("dbo.LensCategories", "CollectifAccountID", "dbo.CollectifAccounts");
            DropForeignKey("dbo.LensCategories", "LensMaterialID", "dbo.LensMaterials");
            DropForeignKey("dbo.LensCategories", "CategoryID", "dbo.Categories");
            DropForeignKey("dbo.GenericProducts", "ProductID", "dbo.Products");
            DropForeignKey("dbo.CustomerSlices", "SaleID", "dbo.Sales");
            DropForeignKey("dbo.CustomerSlices", "SliceID", "dbo.Slices");
            DropForeignKey("dbo.Customers", "AccountID", "dbo.Accounts");
            DropForeignKey("dbo.Customers", "GlobalPersonID", "dbo.People");
            DropForeignKey("dbo.People", "SexID", "dbo.Sexes");
            DropForeignKey("dbo.People", "Adress_AdressID", "dbo.Adresses");
            DropForeignKey("dbo.People", "GlobalPersonID", "dbo.GlobalPeople");
            DropForeignKey("dbo.CustomerOrderLines", "CustomerOrderID", "dbo.CustomerOrders");
            DropForeignKey("dbo.CustomerOrderLines", "LineID", "dbo.Lines");
            DropForeignKey("dbo.Companies", "GlobalPersonID", "dbo.GlobalPeople");
            DropForeignKey("dbo.BankSales", "BankID", "dbo.Banks");
            DropForeignKey("dbo.BankSales", "SaleID", "dbo.Sales");
            DropForeignKey("dbo.BankPurchases", "BankID", "dbo.Banks");
            DropForeignKey("dbo.BankPurchases", "PurchaseID", "dbo.Purchases");
            DropForeignKey("dbo.AccountingSpecificTask", "AccountingTaskID", "dbo.AccountingTask");
            DropForeignKey("dbo.AccountingSpecificTask", "TransportAccountID", "dbo.Accounts");
            DropForeignKey("dbo.AccountingSpecificTask", "DiscountAccountID", "dbo.Accounts");
            DropForeignKey("dbo.AccountingSpecificTask", "VatAccountID", "dbo.Accounts");
            DropForeignKey("dbo.AccountingSpecificTask", "AccountID", "dbo.Accounts");
            DropForeignKey("dbo.UserTills", "UserID", "dbo.Users");
            DropForeignKey("dbo.UserTills", "TillID", "dbo.Tills");
            DropForeignKey("dbo.TillDays", "TillID", "dbo.Tills");
            DropForeignKey("dbo.SupplierOrders", "SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.SupplierOrders", "DeviseID", "dbo.Devises");
            DropForeignKey("dbo.SupplierOrders", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.Lines", "ProductID", "dbo.Products");
            DropForeignKey("dbo.Lines", "LocalizationID", "dbo.Localizations");
            DropForeignKey("dbo.LensNumberRangePrices", "SphericalValueRangeID", "dbo.LensNumberRanges");
            DropForeignKey("dbo.LensNumberRangePrices", "LensCategoryID", "dbo.LensCategories");
            DropForeignKey("dbo.LensNumberRangePrices", "CylindricalValueRangeID", "dbo.LensNumberRanges");
            DropForeignKey("dbo.LensNumberRangePrices", "AdditionValueRangeID", "dbo.LensNumberRanges");
            DropForeignKey("dbo.InventoryHistorics", "RegisteredByID", "dbo.Users");
            DropForeignKey("dbo.InventoryHistorics", "ProductID", "dbo.Products");
            DropForeignKey("dbo.InventoryHistorics", "LocalizationID", "dbo.Localizations");
            DropForeignKey("dbo.InventoryHistorics", "CountByID", "dbo.Users");
            DropForeignKey("dbo.InventoryHistorics", "AutorizedByID", "dbo.Users");
            DropForeignKey("dbo.InventoryDirectories", "RegisteredByID", "dbo.Users");
            DropForeignKey("dbo.InventoryDirectoryLines", "RegisteredByID", "dbo.Users");
            DropForeignKey("dbo.InventoryDirectoryLines", "ProductID", "dbo.Products");
            DropForeignKey("dbo.InventoryDirectoryLines", "LocalizationID", "dbo.Localizations");
            DropForeignKey("dbo.InventoryDirectoryLines", "InventoryDirectoryID", "dbo.InventoryDirectories");
            DropForeignKey("dbo.InventoryDirectoryLines", "CountByID", "dbo.Users");
            DropForeignKey("dbo.InventoryDirectoryLines", "AutorizedByID", "dbo.Users");
            DropForeignKey("dbo.InventoryDirectories", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.EmployeeStocks", "WareHouseManID", "dbo.Users");
            DropForeignKey("dbo.EmployeeStocks", "WareHouseID", "dbo.Localizations");
            DropForeignKey("dbo.EmployeeStockHistorics", "WareHouseManID", "dbo.Users");
            DropForeignKey("dbo.EmployeeStockHistorics", "WareHouseID", "dbo.Localizations");
            DropForeignKey("dbo.BusinessDays", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.AccountingSections", "ClassAccountID", "dbo.ClassAccounts");
            DropForeignKey("dbo.AccountingTask", "AccountingSectionID", "dbo.AccountingSections");
            DropForeignKey("dbo.AccountOperations", "Account_AccountID", "dbo.Accounts");
            DropForeignKey("dbo.AccountOperations", "TillAdjustID", "dbo.TillAdjusts");
            DropForeignKey("dbo.TillAdjusts", "TillID", "dbo.Tills");
            DropForeignKey("dbo.TillAdjusts", "DeviseID", "dbo.Devises");
            DropForeignKey("dbo.CustomerReturns", "SaleID", "dbo.Sales");
            DropForeignKey("dbo.CustomerReturnLines", "SaleLineID", "dbo.SaleLines");
            DropForeignKey("dbo.CustomerReturnLines", "CustomerReturnID", "dbo.CustomerReturns");
            DropForeignKey("dbo.SupplierReturnLines", "SupplierReturnID", "dbo.SupplierReturns");
            DropForeignKey("dbo.SupplierReturnLines", "PurchaseLineID", "dbo.PurchaseLines");
            DropForeignKey("dbo.SupplierReturns", "PurchaseID", "dbo.Purchases");
            DropForeignKey("dbo.ProductTransferts", "RegisteredByID", "dbo.Users");
            DropForeignKey("dbo.ProductTransferts", "ReceivedByID", "dbo.Users");
            DropForeignKey("dbo.ProductTransfertLines", "ProductTransfertID", "dbo.ProductTransferts");
            DropForeignKey("dbo.ProductTransfertLines", "ProductID", "dbo.Products");
            DropForeignKey("dbo.ProductTransfertLines", "DepartureLocalizationID", "dbo.Localizations");
            DropForeignKey("dbo.ProductTransfertLines", "ArrivalLocalizationID", "dbo.Localizations");
            DropForeignKey("dbo.ProductTransferts", "OrderedByID", "dbo.Users");
            DropForeignKey("dbo.ProductTransferts", "DepartureBranchID", "dbo.Branches");
            DropForeignKey("dbo.ProductTransferts", "AskedByID", "dbo.Users");
            DropForeignKey("dbo.ProductTransferts", "ArrivalBranchID", "dbo.Branches");
            DropForeignKey("dbo.BudgetConsumptions", "PaymentMethodID", "dbo.PaymentMethods");
            DropForeignKey("dbo.BudgetConsumptions", "DeviseID", "dbo.Devises");
            DropForeignKey("dbo.BudgetConsumptions", "BudgetAllocatedID", "dbo.BudgetAllocateds");
            DropForeignKey("dbo.BudgetAllocateds", "FiscalYearID", "dbo.FiscalYears");
            DropForeignKey("dbo.BudgetAllocateds", "BudgetLineID", "dbo.BudgetLines");
            DropForeignKey("dbo.BudgetLines", "AccountID", "dbo.Accounts");
            DropForeignKey("dbo.BudgetAllocatedUpdates", "BudgetAllocatedID", "dbo.BudgetAllocateds");
            DropForeignKey("dbo.BudgetAllocateds", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.AccountOperations", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.BranchClosingDayTasks", "ClosingDayTaskID", "dbo.ClosingDayTasks");
            DropForeignKey("dbo.BranchClosingDayTasks", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.Branches", "AdressID", "dbo.Adresses");
            DropForeignKey("dbo.Deposits", "SavingAccountID", "dbo.SavingAccounts");
            DropForeignKey("dbo.Purchases", "SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.Purchases", "PurchaseRegisterID", "dbo.Users");
            DropForeignKey("dbo.Purchases", "PurchaseBringerID", "dbo.Users");
            DropForeignKey("dbo.Purchases", "DeviseID", "dbo.Devises");
            DropForeignKey("dbo.Purchases", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.Operations", "ReglementTypeID", "dbo.ReglementTypes");
            DropForeignKey("dbo.Pieces", "OperationID", "dbo.Operations");
            DropForeignKey("dbo.Pieces", "DeviseID", "dbo.Devises");
            DropForeignKey("dbo.Pieces", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.Pieces", "AccountID", "dbo.Accounts");
            DropForeignKey("dbo.Operations", "OperationTypeID", "dbo.OperationTypes");
            DropForeignKey("dbo.Operations", "MacroOperationID", "dbo.MacroOperations");
            DropForeignKey("dbo.AccountOperations", "OperationID", "dbo.Operations");
            DropForeignKey("dbo.AccountingTask", "OperationID", "dbo.Operations");
            DropForeignKey("dbo.Sales", "DeviseID", "dbo.Devises");
            DropForeignKey("dbo.Sales", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.Sales", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.Slices", "PaymentMethodID", "dbo.PaymentMethods");
            DropForeignKey("dbo.Slices", "DeviseID", "dbo.Devises");
            DropForeignKey("dbo.Deposits", "PaymentMethodID", "dbo.PaymentMethods");
            DropForeignKey("dbo.Deposits", "DeviseID", "dbo.Devises");
            DropForeignKey("dbo.PaymentMethods", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.CustomerOrders", "OperatorID", "dbo.Users");
            DropForeignKey("dbo.CustomerOrders", "DeviseID", "dbo.Devises");
            DropForeignKey("dbo.AccountOperations", "DeviseID", "dbo.Devises");
            DropForeignKey("dbo.Localizations", "QuarterID", "dbo.Quarters");
            DropForeignKey("dbo.Towns", "RegionID", "dbo.Regions");
            DropForeignKey("dbo.Regions", "CountryID", "dbo.Countries");
            DropForeignKey("dbo.Quarters", "TownID", "dbo.Towns");
            DropForeignKey("dbo.Adresses", "QuarterID", "dbo.Quarters");
            DropForeignKey("dbo.ProductLocalizations", "ProductID", "dbo.Products");
            DropForeignKey("dbo.Accounts", "CollectifAccountID", "dbo.CollectifAccounts");
            DropForeignKey("dbo.CollectifAccounts", "AccountingSectionID", "dbo.AccountingSections");
            DropForeignKey("dbo.Products", "CategoryID", "dbo.Categories");
            DropForeignKey("dbo.Products", "AccountID", "dbo.Accounts");
            DropForeignKey("dbo.ProductLocalizations", "LocalizationID", "dbo.Localizations");
            DropForeignKey("dbo.Localizations", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.CustomerOrders", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.CustomerOrders", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.UserConfigurations", "DefaultBranchID", "dbo.Branches");
            DropForeignKey("dbo.UserBranches", "UserID", "dbo.Users");
            DropForeignKey("dbo.UserBranches", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.ActionSubMenuProfiles", "SubMenuID", "dbo.SubMenus");
            DropForeignKey("dbo.ActionSubMenuProfiles", "ProfileID", "dbo.Profiles");
            DropForeignKey("dbo.ActionMenuProfiles", "ProfileID", "dbo.Profiles");
            DropForeignKey("dbo.ActionMenuProfiles", "MenuID", "dbo.Menus");
            DropForeignKey("dbo.SubMenus", "MenuID", "dbo.Menus");
            DropForeignKey("dbo.Menus", "ModuleID", "dbo.Modules");
            DropForeignKey("dbo.Jobs", "CompanyID", "dbo.Companies");
            DropForeignKey("dbo.Branches", "CompanyID", "dbo.Companies");
            DropForeignKey("dbo.Files", "GlobalPersonID", "dbo.GlobalPeople");
            DropForeignKey("dbo.GlobalPeople", "AdressID", "dbo.Adresses");
            DropForeignKey("dbo.AccountOperations", "AccountID", "dbo.Accounts");
            DropForeignKey("dbo.AccountingTask", "Account_AccountID", "dbo.Accounts");
            DropIndex("dbo.BudgetConsumptionAccountOperations", new[] { "BudgetConsumptionID" });
            DropIndex("dbo.BudgetConsumptionAccountOperations", new[] { "AccountOperationID" });
            DropIndex("dbo.ProductLocalizationAccountOperations", new[] { "ProductLocalizationID" });
            DropIndex("dbo.ProductLocalizationAccountOperations", new[] { "AccountOperationID" });
            DropIndex("dbo.PurchaseReturnAccountOperations", new[] { "SupplierReturnID" });
            DropIndex("dbo.PurchaseReturnAccountOperations", new[] { "AccountOperationID" });
            DropIndex("dbo.SaleReturnAccountOperations", new[] { "CustomerReturnID" });
            DropIndex("dbo.SaleReturnAccountOperations", new[] { "AccountOperationID" });
            DropIndex("dbo.ProductTransferAccountOperations", new[] { "ProductTransfertID" });
            DropIndex("dbo.ProductTransferAccountOperations", new[] { "AccountOperationID" });
            DropIndex("dbo.PurchaseAccountOperations", new[] { "PurchaseID" });
            DropIndex("dbo.PurchaseAccountOperations", new[] { "AccountOperationID" });
            DropIndex("dbo.SaleAccountOperations", new[] { "SaleID" });
            DropIndex("dbo.SaleAccountOperations", new[] { "AccountOperationID" });
            DropIndex("dbo.ManualAccountOperations", new[] { "PieceID" });
            DropIndex("dbo.ManualAccountOperations", new[] { "AccountOperationID" });
            DropIndex("dbo.Tills", new[] { "AccountID" });
            DropIndex("dbo.Tills", new[] { "ID" });
            DropIndex("dbo.Banks", new[] { "AccountID" });
            DropIndex("dbo.Banks", new[] { "ID" });
            DropIndex("dbo.Users", new[] { "JobID" });
            DropIndex("dbo.Users", new[] { "UserConfigurationID" });
            DropIndex("dbo.Users", new[] { "ProfileID" });
            DropIndex("dbo.Users", "UserLogin");
            DropIndex("dbo.Users", "Code");
            DropIndex("dbo.Users", new[] { "GlobalPersonID" });
            DropIndex("dbo.TillSales", new[] { "TillID" });
            DropIndex("dbo.TillSales", new[] { "SaleID" });
            DropIndex("dbo.TillPurchases", new[] { "TillID" });
            DropIndex("dbo.TillPurchases", new[] { "PurchaseID" });
            DropIndex("dbo.SupplierSlices", new[] { "PurchaseID" });
            DropIndex("dbo.SupplierSlices", new[] { "SliceID" });
            DropIndex("dbo.Suppliers", "SupplierNumber");
            DropIndex("dbo.Suppliers", new[] { "AccountID" });
            DropIndex("dbo.Suppliers", new[] { "GlobalPersonID" });
            DropIndex("dbo.SupplierOrderLines", new[] { "SupplierOrderID" });
            DropIndex("dbo.SupplierOrderLines", new[] { "LineID" });
            DropIndex("dbo.SpecialOrderSlices", new[] { "SpecialOrderID" });
            DropIndex("dbo.SpecialOrderSlices", new[] { "SliceID" });
            DropIndex("dbo.SpecialOrders", new[] { "CustomerOrderID" });
            DropIndex("dbo.SavingAccountSales", new[] { "SavingAccountID" });
            DropIndex("dbo.SavingAccountSales", new[] { "SaleID" });
            DropIndex("dbo.SavingAccounts", new[] { "CustomerID" });
            DropIndex("dbo.SavingAccounts", new[] { "ID" });
            DropIndex("dbo.SaleLines", new[] { "SaleID" });
            DropIndex("dbo.SaleLines", new[] { "LineID" });
            DropIndex("dbo.PurchaseLines", new[] { "PurchaseID" });
            DropIndex("dbo.PurchaseLines", new[] { "LineID" });
            DropIndex("dbo.OrderLenses", new[] { "LensNumberID" });
            DropIndex("dbo.OrderLenses", new[] { "LensCategoryID" });
            DropIndex("dbo.OrderLenses", new[] { "ProductID" });
            DropIndex("dbo.Lenses", new[] { "LensNumberID" });
            DropIndex("dbo.Lenses", new[] { "LensCategoryID" });
            DropIndex("dbo.Lenses", new[] { "ProductID" });
            DropIndex("dbo.LensCategories", new[] { "LensColourID" });
            DropIndex("dbo.LensCategories", new[] { "LensCoatingID" });
            DropIndex("dbo.LensCategories", new[] { "CollectifAccountID" });
            DropIndex("dbo.LensCategories", new[] { "LensMaterialID" });
            DropIndex("dbo.LensCategories", new[] { "CategoryID" });
            DropIndex("dbo.GenericProducts", new[] { "ProductID" });
            DropIndex("dbo.CustomerSlices", new[] { "SaleID" });
            DropIndex("dbo.CustomerSlices", new[] { "SliceID" });
            DropIndex("dbo.Customers", "CustomerNumber");
            DropIndex("dbo.Customers", new[] { "AccountID" });
            DropIndex("dbo.Customers", new[] { "GlobalPersonID" });
            DropIndex("dbo.People", new[] { "SexID" });
            DropIndex("dbo.People", new[] { "Adress_AdressID" });
            DropIndex("dbo.People", new[] { "GlobalPersonID" });
            DropIndex("dbo.CustomerOrderLines", new[] { "CustomerOrderID" });
            DropIndex("dbo.CustomerOrderLines", new[] { "LineID" });
            DropIndex("dbo.Companies", "CompanyTradeRegister");
            DropIndex("dbo.Companies", "CompanySigle");
            DropIndex("dbo.Companies", new[] { "GlobalPersonID" });
            DropIndex("dbo.BankSales", new[] { "BankID" });
            DropIndex("dbo.BankSales", new[] { "SaleID" });
            DropIndex("dbo.BankPurchases", new[] { "BankID" });
            DropIndex("dbo.BankPurchases", new[] { "PurchaseID" });
            DropIndex("dbo.AccountingSpecificTask", new[] { "TransportAccountID" });
            DropIndex("dbo.AccountingSpecificTask", new[] { "DiscountAccountID" });
            DropIndex("dbo.AccountingSpecificTask", new[] { "VatAccountID" });
            DropIndex("dbo.AccountingSpecificTask", new[] { "AccountID" });
            DropIndex("dbo.AccountingSpecificTask", new[] { "AccountingTaskID" });
            DropIndex("dbo.UserTills", new[] { "UserID" });
            DropIndex("dbo.UserTills", new[] { "TillID" });
            DropIndex("dbo.TransactNumbers", new[] { "TransactNumberCode" });
            DropIndex("dbo.TillDays", new[] { "TillID" });
            DropIndex("dbo.SupplierOrders", new[] { "BranchID" });
            DropIndex("dbo.SupplierOrders", new[] { "DeviseID" });
            DropIndex("dbo.SupplierOrders", new[] { "SupplierID" });
            DropIndex("dbo.SupplierOrders", new[] { "SupplierOrderReference" });
            DropIndex("dbo.LensNumberRanges", "IX_RealPrimaryKey");
            DropIndex("dbo.LensNumberRangePrices", new[] { "AdditionValueRangeID" });
            DropIndex("dbo.LensNumberRangePrices", new[] { "CylindricalValueRangeID" });
            DropIndex("dbo.LensNumberRangePrices", new[] { "SphericalValueRangeID" });
            DropIndex("dbo.LensNumberRangePrices", new[] { "LensCategoryID" });
            DropIndex("dbo.InventoryHistorics", new[] { "RegisteredByID" });
            DropIndex("dbo.InventoryHistorics", new[] { "CountByID" });
            DropIndex("dbo.InventoryHistorics", new[] { "AutorizedByID" });
            DropIndex("dbo.InventoryHistorics", new[] { "LocalizationID" });
            DropIndex("dbo.InventoryHistorics", new[] { "ProductID" });
            DropIndex("dbo.InventoryDirectoryLines", new[] { "InventoryDirectoryID" });
            DropIndex("dbo.InventoryDirectoryLines", new[] { "RegisteredByID" });
            DropIndex("dbo.InventoryDirectoryLines", new[] { "CountByID" });
            DropIndex("dbo.InventoryDirectoryLines", new[] { "AutorizedByID" });
            DropIndex("dbo.InventoryDirectoryLines", new[] { "LocalizationID" });
            DropIndex("dbo.InventoryDirectoryLines", new[] { "ProductID" });
            DropIndex("dbo.InventoryDirectories", new[] { "RegisteredByID" });
            DropIndex("dbo.InventoryDirectories", new[] { "BranchID" });
            DropIndex("dbo.EmployeeStocks", new[] { "WareHouseID" });
            DropIndex("dbo.EmployeeStocks", new[] { "WareHouseManID" });
            DropIndex("dbo.EmployeeStockHistorics", new[] { "WareHouseID" });
            DropIndex("dbo.EmployeeStockHistorics", new[] { "WareHouseManID" });
            DropIndex("dbo.BusinessDays", new[] { "BranchID" });
            DropIndex("dbo.BusinessDays", "BDCode");
            DropIndex("dbo.ClassAccounts", "ClassAccountCode");
            DropIndex("dbo.ClassAccounts", "ClassAccountNumber");
            DropIndex("dbo.TillAdjusts", new[] { "DeviseID" });
            DropIndex("dbo.TillAdjusts", new[] { "TillID" });
            DropIndex("dbo.CustomerReturnLines", new[] { "CustomerReturnID" });
            DropIndex("dbo.CustomerReturnLines", new[] { "SaleLineID" });
            DropIndex("dbo.CustomerReturns", "SaleID_IX");
            DropIndex("dbo.SupplierReturnLines", new[] { "PurchaseLineID" });
            DropIndex("dbo.SupplierReturnLines", new[] { "SupplierReturnID" });
            DropIndex("dbo.SupplierReturns", new[] { "PurchaseID" });
            DropIndex("dbo.ProductTransfertLines", new[] { "ProductTransfertID" });
            DropIndex("dbo.ProductTransfertLines", new[] { "ArrivalLocalizationID" });
            DropIndex("dbo.ProductTransfertLines", new[] { "DepartureLocalizationID" });
            DropIndex("dbo.ProductTransfertLines", new[] { "ProductID" });
            DropIndex("dbo.ProductTransferts", new[] { "ReceivedByID" });
            DropIndex("dbo.ProductTransferts", new[] { "RegisteredByID" });
            DropIndex("dbo.ProductTransferts", new[] { "OrderedByID" });
            DropIndex("dbo.ProductTransferts", new[] { "AskedByID" });
            DropIndex("dbo.ProductTransferts", new[] { "ArrivalBranchID" });
            DropIndex("dbo.ProductTransferts", new[] { "DepartureBranchID" });
            DropIndex("dbo.ProductTransferts", "ProductTransfertReference");
            DropIndex("dbo.FiscalYears", "FiscalYearStatus");
            DropIndex("dbo.FiscalYears", "FiscalYearNumber");
            DropIndex("dbo.BudgetLines", new[] { "AccountID" });
            DropIndex("dbo.BudgetLines", "BudgetCode");
            DropIndex("dbo.BudgetAllocatedUpdates", new[] { "BudgetAllocatedID" });
            DropIndex("dbo.BudgetAllocateds", new[] { "BudgetLineID" });
            DropIndex("dbo.BudgetAllocateds", new[] { "FiscalYearID" });
            DropIndex("dbo.BudgetAllocateds", new[] { "BranchID" });
            DropIndex("dbo.BudgetConsumptions", new[] { "DeviseID" });
            DropIndex("dbo.BudgetConsumptions", new[] { "PaymentMethodID" });
            DropIndex("dbo.BudgetConsumptions", new[] { "BudgetAllocatedID" });
            DropIndex("dbo.ClosingDayTasks", "ClosingDayTaskCode");
            DropIndex("dbo.BranchClosingDayTasks", new[] { "BranchID" });
            DropIndex("dbo.BranchClosingDayTasks", new[] { "ClosingDayTaskID" });
            DropIndex("dbo.Purchases", new[] { "SupplierID" });
            DropIndex("dbo.Purchases", new[] { "PurchaseBringerID" });
            DropIndex("dbo.Purchases", new[] { "DeviseID" });
            DropIndex("dbo.Purchases", new[] { "BranchID" });
            DropIndex("dbo.Purchases", new[] { "PurchaseRegisterID" });
            DropIndex("dbo.Purchases", "PurchaseReference");
            DropIndex("dbo.ReglementTypes", new[] { "ReglementTypeCode" });
            DropIndex("dbo.Pieces", new[] { "AccountID" });
            DropIndex("dbo.Pieces", new[] { "OperationID" });
            DropIndex("dbo.Pieces", new[] { "DeviseID" });
            DropIndex("dbo.Pieces", new[] { "BranchID" });
            DropIndex("dbo.OperationTypes", new[] { "operationTypeCode" });
            DropIndex("dbo.MacroOperations", new[] { "MacroOperationCode" });
            DropIndex("dbo.Operations", new[] { "ReglementTypeID" });
            DropIndex("dbo.Operations", new[] { "MacroOperationID" });
            DropIndex("dbo.Operations", new[] { "OperationTypeID" });
            DropIndex("dbo.Operations", new[] { "OperationCode" });
            DropIndex("dbo.Sales", new[] { "CustomerID" });
            DropIndex("dbo.Sales", new[] { "BranchID" });
            DropIndex("dbo.Sales", new[] { "SaleReceiptNumber" });
            DropIndex("dbo.Sales", new[] { "DeviseID" });
            DropIndex("dbo.Deposits", new[] { "SavingAccountID" });
            DropIndex("dbo.Deposits", new[] { "DeviseID" });
            DropIndex("dbo.Deposits", new[] { "PaymentMethodID" });
            DropIndex("dbo.PaymentMethods", new[] { "BranchID" });
            DropIndex("dbo.PaymentMethods", new[] { "Code" });
            DropIndex("dbo.Slices", new[] { "PaymentMethodID" });
            DropIndex("dbo.Slices", new[] { "DeviseID" });
            DropIndex("dbo.Devises", "DeviseCode");
            DropIndex("dbo.Countries", "CountryCode");
            DropIndex("dbo.Regions", "IX_RealPrimaryKey");
            DropIndex("dbo.Towns", new[] { "RegionID" });
            DropIndex("dbo.Towns", "TownCode");
            DropIndex("dbo.Quarters", new[] { "TownID" });
            DropIndex("dbo.Quarters", "QuarterCode");
            DropIndex("dbo.LensNumbers", "IX_RealPrimaryKey");
            DropIndex("dbo.LensMaterials", "LensMaterialCode");
            DropIndex("dbo.LensColours", "LensColourCode");
            DropIndex("dbo.LensCoatings", "LensCoatingCode");
            DropIndex("dbo.CollectifAccounts", new[] { "AccountingSectionID" });
            DropIndex("dbo.CollectifAccounts", new[] { "CollectifAccountNumber" });
            DropIndex("dbo.Categories", "CategoryCode");
            DropIndex("dbo.Products", new[] { "AccountID" });
            DropIndex("dbo.Products", new[] { "CategoryID" });
            DropIndex("dbo.Products", "ProductCode");
            DropIndex("dbo.ProductLocalizations", "IX_RealPrimaryKey");
            DropIndex("dbo.Localizations", new[] { "BranchID" });
            DropIndex("dbo.Localizations", new[] { "QuarterID" });
            DropIndex("dbo.Localizations", new[] { "LocalizationCode" });
            DropIndex("dbo.Lines", new[] { "ProductID" });
            DropIndex("dbo.Lines", new[] { "LocalizationID" });
            DropIndex("dbo.CustomerOrders", new[] { "OperatorID" });
            DropIndex("dbo.CustomerOrders", new[] { "BranchID" });
            DropIndex("dbo.CustomerOrders", new[] { "DeviseID" });
            DropIndex("dbo.CustomerOrders", new[] { "CustomerID" });
            DropIndex("dbo.CustomerOrders", new[] { "CustomerOrderNumber" });
            DropIndex("dbo.UserConfigurations", new[] { "DefaultBranchID" });
            DropIndex("dbo.UserBranches", new[] { "UserID" });
            DropIndex("dbo.UserBranches", new[] { "BranchID" });
            DropIndex("dbo.ActionSubMenuProfiles", "IX_RealPrimaryKey");
            DropIndex("dbo.SubMenus", new[] { "MenuID" });
            DropIndex("dbo.SubMenus", "SubMenuCode");
            DropIndex("dbo.Modules", "ModuleCode");
            DropIndex("dbo.Menus", new[] { "ModuleID" });
            DropIndex("dbo.Menus", "MenuCode");
            DropIndex("dbo.ActionMenuProfiles", "IX_RealPrimaryKey");
            DropIndex("dbo.Profiles", "ProfileCode");
            DropIndex("dbo.Sexes", new[] { "SexCode" });
            DropIndex("dbo.Jobs", new[] { "CompanyID" });
            DropIndex("dbo.Files", new[] { "GlobalPersonID" });
            DropIndex("dbo.GlobalPeople", new[] { "AdressID" });
            DropIndex("dbo.GlobalPeople", "CNI");
            DropIndex("dbo.Adresses", new[] { "QuarterID" });
            DropIndex("dbo.Branches", new[] { "CompanyID" });
            DropIndex("dbo.Branches", new[] { "AdressID" });
            DropIndex("dbo.Branches", "BranchName");
            DropIndex("dbo.Branches", "BranchCode");
            DropIndex("dbo.AccountOperations", new[] { "Account_AccountID" });
            DropIndex("dbo.AccountOperations", new[] { "TillAdjustID" });
            DropIndex("dbo.AccountOperations", new[] { "DeviseID" });
            DropIndex("dbo.AccountOperations", new[] { "AccountID" });
            DropIndex("dbo.AccountOperations", new[] { "OperationID" });
            DropIndex("dbo.AccountOperations", new[] { "BranchID" });
            DropIndex("dbo.Accounts", new[] { "CollectifAccountID" });
            DropIndex("dbo.Accounts", new[] { "AccountNumber" });
            DropIndex("dbo.AccountingTask", new[] { "Account_AccountID" });
            DropIndex("dbo.AccountingTask", new[] { "AccountingSectionID" });
            DropIndex("dbo.AccountingTask", new[] { "OperationID" });
            DropIndex("dbo.AccountingSections", new[] { "ClassAccountID" });
            DropIndex("dbo.AccountingSections", "CategoryCode");
            DropIndex("dbo.AccountingSections", new[] { "AccountingSectionNumber" });
            DropTable("dbo.BudgetConsumptionAccountOperations");
            DropTable("dbo.ProductLocalizationAccountOperations");
            DropTable("dbo.PurchaseReturnAccountOperations");
            DropTable("dbo.SaleReturnAccountOperations");
            DropTable("dbo.ProductTransferAccountOperations");
            DropTable("dbo.PurchaseAccountOperations");
            DropTable("dbo.SaleAccountOperations");
            DropTable("dbo.ManualAccountOperations");
            DropTable("dbo.Tills");
            DropTable("dbo.Banks");
            DropTable("dbo.Users");
            DropTable("dbo.TillSales");
            DropTable("dbo.TillPurchases");
            DropTable("dbo.SupplierSlices");
            DropTable("dbo.Suppliers");
            DropTable("dbo.SupplierOrderLines");
            DropTable("dbo.SpecialOrderSlices");
            DropTable("dbo.SpecialOrders");
            DropTable("dbo.SavingAccountSales");
            DropTable("dbo.SavingAccounts");
            DropTable("dbo.SaleLines");
            DropTable("dbo.PurchaseLines");
            DropTable("dbo.OrderLenses");
            DropTable("dbo.Lenses");
            DropTable("dbo.LensCategories");
            DropTable("dbo.GenericProducts");
            DropTable("dbo.CustomerSlices");
            DropTable("dbo.Customers");
            DropTable("dbo.People");
            DropTable("dbo.CustomerOrderLines");
            DropTable("dbo.Companies");
            DropTable("dbo.BankSales");
            DropTable("dbo.BankPurchases");
            DropTable("dbo.AccountingSpecificTask");
            DropTable("dbo.UserTills");
            DropTable("dbo.TransactNumbers");
            DropTable("dbo.TillDays");
            DropTable("dbo.Sneaks");
            DropTable("dbo.SneakDays");
            DropTable("dbo.RptReceipts");
            DropTable("dbo.RptPrintStmts");
            DropTable("dbo.RptInventories");
            DropTable("dbo.RptIncomeExpenses");
            DropTable("dbo.RptHeaders");
            DropTable("dbo.RptEtatsJournals");
            DropTable("dbo.RptCustomerPayments");
            DropTable("dbo.RptCashOpHists");
            DropTable("dbo.RptCashDayOperations");
            DropTable("dbo.RptBills");
            DropTable("dbo.RptBalanceGenerales");
            DropTable("dbo.RptAcctingPlans");
            DropTable("dbo.SupplierOrders");
            DropTable("dbo.LensNumberRanges");
            DropTable("dbo.LensNumberRangePrices");
            DropTable("dbo.InventoryHistorics");
            DropTable("dbo.InventoryDirectoryLines");
            DropTable("dbo.InventoryDirectories");
            DropTable("dbo.EmployeeStocks");
            DropTable("dbo.EmployeeStockHistorics");
            DropTable("dbo.BusinessDays");
            DropTable("dbo.ClassAccounts");
            DropTable("dbo.TillAdjusts");
            DropTable("dbo.CustomerReturnLines");
            DropTable("dbo.CustomerReturns");
            DropTable("dbo.SupplierReturnLines");
            DropTable("dbo.SupplierReturns");
            DropTable("dbo.ProductTransfertLines");
            DropTable("dbo.ProductTransferts");
            DropTable("dbo.FiscalYears");
            DropTable("dbo.BudgetLines");
            DropTable("dbo.BudgetAllocatedUpdates");
            DropTable("dbo.BudgetAllocateds");
            DropTable("dbo.BudgetConsumptions");
            DropTable("dbo.ClosingDayTasks");
            DropTable("dbo.BranchClosingDayTasks");
            DropTable("dbo.Purchases");
            DropTable("dbo.ReglementTypes");
            DropTable("dbo.Pieces");
            DropTable("dbo.OperationTypes");
            DropTable("dbo.MacroOperations");
            DropTable("dbo.Operations");
            DropTable("dbo.Sales");
            DropTable("dbo.Deposits");
            DropTable("dbo.PaymentMethods");
            DropTable("dbo.Slices");
            DropTable("dbo.Devises");
            DropTable("dbo.Countries");
            DropTable("dbo.Regions");
            DropTable("dbo.Towns");
            DropTable("dbo.Quarters");
            DropTable("dbo.LensNumbers");
            DropTable("dbo.LensMaterials");
            DropTable("dbo.LensColours");
            DropTable("dbo.LensCoatings");
            DropTable("dbo.CollectifAccounts");
            DropTable("dbo.Categories");
            DropTable("dbo.Products");
            DropTable("dbo.ProductLocalizations");
            DropTable("dbo.Localizations");
            DropTable("dbo.Lines");
            DropTable("dbo.CustomerOrders");
            DropTable("dbo.UserConfigurations");
            DropTable("dbo.UserBranches");
            DropTable("dbo.ActionSubMenuProfiles");
            DropTable("dbo.SubMenus");
            DropTable("dbo.Modules");
            DropTable("dbo.Menus");
            DropTable("dbo.ActionMenuProfiles");
            DropTable("dbo.Profiles");
            DropTable("dbo.Sexes");
            DropTable("dbo.Jobs");
            DropTable("dbo.Files");
            DropTable("dbo.GlobalPeople");
            DropTable("dbo.Adresses");
            DropTable("dbo.Branches");
            DropTable("dbo.AccountOperations");
            DropTable("dbo.Accounts");
            DropTable("dbo.AccountingTask");
            DropTable("dbo.AccountingSections");
        }
    }
}
