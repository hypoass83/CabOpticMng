namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Consultation_Entity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Prescriptions", "CustomerID", "dbo.Customers");
            DropIndex("dbo.Prescriptions", new[] { "CustomerID" });
            CreateTable(
                "dbo.Consultations",
                c => new
                    {
                        ConsultationID = c.Int(nullable: false, identity: true),
                        CustomerID = c.Int(nullable: false),
                        IsNewCustomer = c.Boolean(nullable: false),
                        isPrescritionValidate = c.Boolean(nullable: false),
                        DateConsultation = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ConsultationID)
                .ForeignKey("dbo.Customers", t => t.CustomerID)
                .Index(t => t.CustomerID);
            
            AddColumn("dbo.Sales", "Consultation_ConsultationID", c => c.Int());
            AddColumn("dbo.Prescriptions", "ConsultationID", c => c.Int(nullable: false));
            CreateIndex("dbo.Sales", "Consultation_ConsultationID");
            CreateIndex("dbo.Prescriptions", "ConsultationID");
            AddForeignKey("dbo.Sales", "Consultation_ConsultationID", "dbo.Consultations", "ConsultationID");
            AddForeignKey("dbo.Prescriptions", "ConsultationID", "dbo.Consultations", "ConsultationID");
            DropColumn("dbo.Customers", "isPrescritionValidate");
            DropColumn("dbo.Prescriptions", "CustomerID");
            CreateStoredProcedure(
                "dbo.Consultation_Insert",
                p => new
                    {
                        CustomerID = p.Int(),
                        IsNewCustomer = p.Boolean(),
                        isPrescritionValidate = p.Boolean(),
                        DateConsultation = p.DateTime(),
                    },
                body:
                    @"INSERT [dbo].[Consultations]([CustomerID], [IsNewCustomer], [isPrescritionValidate], [DateConsultation])
                      VALUES (@CustomerID, @IsNewCustomer, @isPrescritionValidate, @DateConsultation)
                      
                      DECLARE @ConsultationID int
                      SELECT @ConsultationID = [ConsultationID]
                      FROM [dbo].[Consultations]
                      WHERE @@ROWCOUNT > 0 AND [ConsultationID] = scope_identity()
                      
                      SELECT t0.[ConsultationID]
                      FROM [dbo].[Consultations] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[ConsultationID] = @ConsultationID"
            );
            
            CreateStoredProcedure(
                "dbo.Consultation_Update",
                p => new
                    {
                        ConsultationID = p.Int(),
                        CustomerID = p.Int(),
                        IsNewCustomer = p.Boolean(),
                        isPrescritionValidate = p.Boolean(),
                        DateConsultation = p.DateTime(),
                    },
                body:
                    @"UPDATE [dbo].[Consultations]
                      SET [CustomerID] = @CustomerID, [IsNewCustomer] = @IsNewCustomer, [isPrescritionValidate] = @isPrescritionValidate, [DateConsultation] = @DateConsultation
                      WHERE ([ConsultationID] = @ConsultationID)"
            );
            
            CreateStoredProcedure(
                "dbo.Consultation_Delete",
                p => new
                    {
                        ConsultationID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Consultations]
                      WHERE ([ConsultationID] = @ConsultationID)"
            );
            
            AlterStoredProcedure(
                "dbo.Customer_Insert",
                p => new
                    {
                        Name = p.String(),
                        Tiergroup = p.String(),
                        Description = p.String(),
                        CNI = p.String(maxLength: 100),
                        AdressID = p.Int(),
                        IsConnected = p.Boolean(),
                        SexID = p.Int(),
                        AccountID = p.Int(),
                        IsCashCustomer = p.Int(),
                        PoliceAssurance = p.String(maxLength: 250),
                        CompanyName = p.String(maxLength: 250),
                        DateOfBirth = p.DateTime(),
                        IsBillCustomer = p.Boolean(),
                        CustomerNumber = p.String(maxLength: 10),
                        Profession = p.String(maxLength: 250),
                        AssureurID = p.Int(),
                        GestionnaireID = p.Int(),
                        LimitAmount = p.Double(),
                        Dateregister = p.DateTime(),
                        Adress_AdressID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[GlobalPeople]([Name], [Tiergroup], [Description], [CNI], [AdressID])
                      VALUES (@Name, @Tiergroup, @Description, @CNI, @AdressID)
                      
                      DECLARE @GlobalPersonID int
                      SELECT @GlobalPersonID = [GlobalPersonID]
                      FROM [dbo].[GlobalPeople]
                      WHERE @@ROWCOUNT > 0 AND [GlobalPersonID] = scope_identity()
                      
                      INSERT [dbo].[People]([GlobalPersonID], [Adress_AdressID], [IsConnected], [SexID])
                      VALUES (@GlobalPersonID, @Adress_AdressID, @IsConnected, @SexID)
                      
                      INSERT [dbo].[Customers]([GlobalPersonID], [AccountID], [IsCashCustomer], [PoliceAssurance], [CompanyName], [DateOfBirth], [IsBillCustomer], [CustomerNumber], [Profession], [AssureurID], [GestionnaireID], [LimitAmount], [Dateregister])
                      VALUES (@GlobalPersonID, @AccountID, @IsCashCustomer, @PoliceAssurance, @CompanyName, @DateOfBirth, @IsBillCustomer, @CustomerNumber, @Profession, @AssureurID, @GestionnaireID, @LimitAmount, @Dateregister)
                      
                      SELECT t0.[GlobalPersonID]
                      FROM [dbo].[GlobalPeople] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[GlobalPersonID] = @GlobalPersonID"
            );
            
            AlterStoredProcedure(
                "dbo.Customer_Update",
                p => new
                    {
                        GlobalPersonID = p.Int(),
                        Name = p.String(),
                        Tiergroup = p.String(),
                        Description = p.String(),
                        CNI = p.String(maxLength: 100),
                        AdressID = p.Int(),
                        IsConnected = p.Boolean(),
                        SexID = p.Int(),
                        AccountID = p.Int(),
                        IsCashCustomer = p.Int(),
                        PoliceAssurance = p.String(maxLength: 250),
                        CompanyName = p.String(maxLength: 250),
                        DateOfBirth = p.DateTime(),
                        IsBillCustomer = p.Boolean(),
                        CustomerNumber = p.String(maxLength: 10),
                        Profession = p.String(maxLength: 250),
                        AssureurID = p.Int(),
                        GestionnaireID = p.Int(),
                        LimitAmount = p.Double(),
                        Dateregister = p.DateTime(),
                        Adress_AdressID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Customers]
                      SET [AccountID] = @AccountID, [IsCashCustomer] = @IsCashCustomer, [PoliceAssurance] = @PoliceAssurance, [CompanyName] = @CompanyName, [DateOfBirth] = @DateOfBirth, [IsBillCustomer] = @IsBillCustomer, [CustomerNumber] = @CustomerNumber, [Profession] = @Profession, [AssureurID] = @AssureurID, [GestionnaireID] = @GestionnaireID, [LimitAmount] = @LimitAmount, [Dateregister] = @Dateregister
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      
                      UPDATE [dbo].[GlobalPeople]
                      SET [Name] = @Name, [Tiergroup] = @Tiergroup, [Description] = @Description, [CNI] = @CNI, [AdressID] = @AdressID
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0
                      
                      UPDATE [dbo].[People]
                      SET [Adress_AdressID] = @Adress_AdressID, [IsConnected] = @IsConnected, [SexID] = @SexID
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0"
            );
            
            AlterStoredProcedure(
                "dbo.Sale_Insert",
                p => new
                    {
                        CompteurFacture = p.Int(),
                        SaleDeliver = p.Boolean(),
                        VatRate = p.Double(),
                        RateReduction = p.Double(),
                        RateDiscount = p.Double(),
                        Transport = p.Double(),
                        SaleDeliveryDate = p.DateTime(),
                        SaleDate = p.DateTime(),
                        SaleDateHours = p.DateTime(),
                        SaleValidate = p.Boolean(),
                        PoliceAssurance = p.String(),
                        PaymentDelay = p.Int(),
                        Guaranteed = p.Int(),
                        Patient = p.String(),
                        DeviseID = p.Int(),
                        IsPaid = p.Boolean(),
                        SaleReceiptNumber = p.String(maxLength: 100),
                        BranchID = p.Int(),
                        CustomerID = p.Int(),
                        CustomerName = p.String(),
                        isReturn = p.Boolean(),
                        StatutSale = p.Int(),
                        OperatorID = p.Int(),
                        PostByID = p.Int(),
                        IsSpecialOrder = p.Boolean(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        CustomerOrderID = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        DateRdv = p.DateTime(),
                        IsRendezVous = p.Boolean(),
                        AwaitingDay = p.Int(),
                        IsProductReveive = p.Boolean(),
                        EffectiveReceiveDate = p.DateTime(),
                        IsCustomerRceive = p.Boolean(),
                        CustomerDeliverDate = p.DateTime(),
                        PostSOByID = p.Int(),
                        ReceiveSOByID = p.Int(),
                        CNI = p.String(maxLength: 100),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [CNI], [Consultation_ConsultationID])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @CNI, @Consultation_ConsultationID)
                      
                      DECLARE @SaleID int
                      SELECT @SaleID = [SaleID]
                      FROM [dbo].[Sales]
                      WHERE @@ROWCOUNT > 0 AND [SaleID] = scope_identity()
                      
                      SELECT t0.[SaleID]
                      FROM [dbo].[Sales] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[SaleID] = @SaleID"
            );
            
            AlterStoredProcedure(
                "dbo.Sale_Update",
                p => new
                    {
                        SaleID = p.Int(),
                        CompteurFacture = p.Int(),
                        SaleDeliver = p.Boolean(),
                        VatRate = p.Double(),
                        RateReduction = p.Double(),
                        RateDiscount = p.Double(),
                        Transport = p.Double(),
                        SaleDeliveryDate = p.DateTime(),
                        SaleDate = p.DateTime(),
                        SaleDateHours = p.DateTime(),
                        SaleValidate = p.Boolean(),
                        PoliceAssurance = p.String(),
                        PaymentDelay = p.Int(),
                        Guaranteed = p.Int(),
                        Patient = p.String(),
                        DeviseID = p.Int(),
                        IsPaid = p.Boolean(),
                        SaleReceiptNumber = p.String(maxLength: 100),
                        BranchID = p.Int(),
                        CustomerID = p.Int(),
                        CustomerName = p.String(),
                        isReturn = p.Boolean(),
                        StatutSale = p.Int(),
                        OperatorID = p.Int(),
                        PostByID = p.Int(),
                        IsSpecialOrder = p.Boolean(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        CustomerOrderID = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        DateRdv = p.DateTime(),
                        IsRendezVous = p.Boolean(),
                        AwaitingDay = p.Int(),
                        IsProductReveive = p.Boolean(),
                        EffectiveReceiveDate = p.DateTime(),
                        IsCustomerRceive = p.Boolean(),
                        CustomerDeliverDate = p.DateTime(),
                        PostSOByID = p.Int(),
                        ReceiveSOByID = p.Int(),
                        CNI = p.String(maxLength: 100),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Sales]
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [CNI] = @CNI, [Consultation_ConsultationID] = @Consultation_ConsultationID
                      WHERE ([SaleID] = @SaleID)"
            );
            
            AlterStoredProcedure(
                "dbo.Sale_Delete",
                p => new
                    {
                        SaleID = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Sales]
                      WHERE (([SaleID] = @SaleID) AND (([Consultation_ConsultationID] = @Consultation_ConsultationID) OR ([Consultation_ConsultationID] IS NULL AND @Consultation_ConsultationID IS NULL)))"
            );
            
            AlterStoredProcedure(
                "dbo.BankSale_Insert",
                p => new
                    {
                        CompteurFacture = p.Int(),
                        SaleDeliver = p.Boolean(),
                        VatRate = p.Double(),
                        RateReduction = p.Double(),
                        RateDiscount = p.Double(),
                        Transport = p.Double(),
                        SaleDeliveryDate = p.DateTime(),
                        SaleDate = p.DateTime(),
                        SaleDateHours = p.DateTime(),
                        SaleValidate = p.Boolean(),
                        PoliceAssurance = p.String(),
                        PaymentDelay = p.Int(),
                        Guaranteed = p.Int(),
                        Patient = p.String(),
                        DeviseID = p.Int(),
                        IsPaid = p.Boolean(),
                        SaleReceiptNumber = p.String(maxLength: 100),
                        BranchID = p.Int(),
                        CustomerID = p.Int(),
                        CustomerName = p.String(),
                        isReturn = p.Boolean(),
                        StatutSale = p.Int(),
                        OperatorID = p.Int(),
                        PostByID = p.Int(),
                        IsSpecialOrder = p.Boolean(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        CustomerOrderID = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        DateRdv = p.DateTime(),
                        IsRendezVous = p.Boolean(),
                        AwaitingDay = p.Int(),
                        IsProductReveive = p.Boolean(),
                        EffectiveReceiveDate = p.DateTime(),
                        IsCustomerRceive = p.Boolean(),
                        CustomerDeliverDate = p.DateTime(),
                        PostSOByID = p.Int(),
                        ReceiveSOByID = p.Int(),
                        CNI = p.String(maxLength: 100),
                        BankID = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [CNI], [Consultation_ConsultationID])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @CNI, @Consultation_ConsultationID)
                      
                      DECLARE @SaleID int
                      SELECT @SaleID = [SaleID]
                      FROM [dbo].[Sales]
                      WHERE @@ROWCOUNT > 0 AND [SaleID] = scope_identity()
                      
                      INSERT [dbo].[BankSales]([SaleID], [BankID])
                      VALUES (@SaleID, @BankID)
                      
                      SELECT t0.[SaleID]
                      FROM [dbo].[Sales] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[SaleID] = @SaleID"
            );
            
            AlterStoredProcedure(
                "dbo.BankSale_Update",
                p => new
                    {
                        SaleID = p.Int(),
                        CompteurFacture = p.Int(),
                        SaleDeliver = p.Boolean(),
                        VatRate = p.Double(),
                        RateReduction = p.Double(),
                        RateDiscount = p.Double(),
                        Transport = p.Double(),
                        SaleDeliveryDate = p.DateTime(),
                        SaleDate = p.DateTime(),
                        SaleDateHours = p.DateTime(),
                        SaleValidate = p.Boolean(),
                        PoliceAssurance = p.String(),
                        PaymentDelay = p.Int(),
                        Guaranteed = p.Int(),
                        Patient = p.String(),
                        DeviseID = p.Int(),
                        IsPaid = p.Boolean(),
                        SaleReceiptNumber = p.String(maxLength: 100),
                        BranchID = p.Int(),
                        CustomerID = p.Int(),
                        CustomerName = p.String(),
                        isReturn = p.Boolean(),
                        StatutSale = p.Int(),
                        OperatorID = p.Int(),
                        PostByID = p.Int(),
                        IsSpecialOrder = p.Boolean(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        CustomerOrderID = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        DateRdv = p.DateTime(),
                        IsRendezVous = p.Boolean(),
                        AwaitingDay = p.Int(),
                        IsProductReveive = p.Boolean(),
                        EffectiveReceiveDate = p.DateTime(),
                        IsCustomerRceive = p.Boolean(),
                        CustomerDeliverDate = p.DateTime(),
                        PostSOByID = p.Int(),
                        ReceiveSOByID = p.Int(),
                        CNI = p.String(maxLength: 100),
                        BankID = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[BankSales]
                      SET [BankID] = @BankID
                      WHERE ([SaleID] = @SaleID)
                      
                      UPDATE [dbo].[Sales]
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [CNI] = @CNI, [Consultation_ConsultationID] = @Consultation_ConsultationID
                      WHERE ([SaleID] = @SaleID)
                      AND @@ROWCOUNT > 0"
            );
            
            AlterStoredProcedure(
                "dbo.BankSale_Delete",
                p => new
                    {
                        SaleID = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[BankSales]
                      WHERE ([SaleID] = @SaleID)
                      
                      DELETE [dbo].[Sales]
                      WHERE (([SaleID] = @SaleID) AND (([Consultation_ConsultationID] = @Consultation_ConsultationID) OR ([Consultation_ConsultationID] IS NULL AND @Consultation_ConsultationID IS NULL)))
                      AND @@ROWCOUNT > 0"
            );
            
            AlterStoredProcedure(
                "dbo.TillSale_Insert",
                p => new
                    {
                        CompteurFacture = p.Int(),
                        SaleDeliver = p.Boolean(),
                        VatRate = p.Double(),
                        RateReduction = p.Double(),
                        RateDiscount = p.Double(),
                        Transport = p.Double(),
                        SaleDeliveryDate = p.DateTime(),
                        SaleDate = p.DateTime(),
                        SaleDateHours = p.DateTime(),
                        SaleValidate = p.Boolean(),
                        PoliceAssurance = p.String(),
                        PaymentDelay = p.Int(),
                        Guaranteed = p.Int(),
                        Patient = p.String(),
                        DeviseID = p.Int(),
                        IsPaid = p.Boolean(),
                        SaleReceiptNumber = p.String(maxLength: 100),
                        BranchID = p.Int(),
                        CustomerID = p.Int(),
                        CustomerName = p.String(),
                        isReturn = p.Boolean(),
                        StatutSale = p.Int(),
                        OperatorID = p.Int(),
                        PostByID = p.Int(),
                        IsSpecialOrder = p.Boolean(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        CustomerOrderID = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        DateRdv = p.DateTime(),
                        IsRendezVous = p.Boolean(),
                        AwaitingDay = p.Int(),
                        IsProductReveive = p.Boolean(),
                        EffectiveReceiveDate = p.DateTime(),
                        IsCustomerRceive = p.Boolean(),
                        CustomerDeliverDate = p.DateTime(),
                        PostSOByID = p.Int(),
                        ReceiveSOByID = p.Int(),
                        CNI = p.String(maxLength: 100),
                        TillID = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [CNI], [Consultation_ConsultationID])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @CNI, @Consultation_ConsultationID)
                      
                      DECLARE @SaleID int
                      SELECT @SaleID = [SaleID]
                      FROM [dbo].[Sales]
                      WHERE @@ROWCOUNT > 0 AND [SaleID] = scope_identity()
                      
                      INSERT [dbo].[TillSales]([SaleID], [TillID])
                      VALUES (@SaleID, @TillID)
                      
                      SELECT t0.[SaleID]
                      FROM [dbo].[Sales] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[SaleID] = @SaleID"
            );
            
            AlterStoredProcedure(
                "dbo.TillSale_Update",
                p => new
                    {
                        SaleID = p.Int(),
                        CompteurFacture = p.Int(),
                        SaleDeliver = p.Boolean(),
                        VatRate = p.Double(),
                        RateReduction = p.Double(),
                        RateDiscount = p.Double(),
                        Transport = p.Double(),
                        SaleDeliveryDate = p.DateTime(),
                        SaleDate = p.DateTime(),
                        SaleDateHours = p.DateTime(),
                        SaleValidate = p.Boolean(),
                        PoliceAssurance = p.String(),
                        PaymentDelay = p.Int(),
                        Guaranteed = p.Int(),
                        Patient = p.String(),
                        DeviseID = p.Int(),
                        IsPaid = p.Boolean(),
                        SaleReceiptNumber = p.String(maxLength: 100),
                        BranchID = p.Int(),
                        CustomerID = p.Int(),
                        CustomerName = p.String(),
                        isReturn = p.Boolean(),
                        StatutSale = p.Int(),
                        OperatorID = p.Int(),
                        PostByID = p.Int(),
                        IsSpecialOrder = p.Boolean(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        CustomerOrderID = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        DateRdv = p.DateTime(),
                        IsRendezVous = p.Boolean(),
                        AwaitingDay = p.Int(),
                        IsProductReveive = p.Boolean(),
                        EffectiveReceiveDate = p.DateTime(),
                        IsCustomerRceive = p.Boolean(),
                        CustomerDeliverDate = p.DateTime(),
                        PostSOByID = p.Int(),
                        ReceiveSOByID = p.Int(),
                        CNI = p.String(maxLength: 100),
                        TillID = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Sales]
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [CNI] = @CNI, [Consultation_ConsultationID] = @Consultation_ConsultationID
                      WHERE ([SaleID] = @SaleID)
                      
                      UPDATE [dbo].[TillSales]
                      SET [TillID] = @TillID
                      WHERE ([SaleID] = @SaleID)
                      AND @@ROWCOUNT > 0"
            );
            
            AlterStoredProcedure(
                "dbo.TillSale_Delete",
                p => new
                    {
                        SaleID = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[TillSales]
                      WHERE ([SaleID] = @SaleID)
                      
                      DELETE [dbo].[Sales]
                      WHERE (([SaleID] = @SaleID) AND (([Consultation_ConsultationID] = @Consultation_ConsultationID) OR ([Consultation_ConsultationID] IS NULL AND @Consultation_ConsultationID IS NULL)))
                      AND @@ROWCOUNT > 0"
            );
            
            AlterStoredProcedure(
                "dbo.AssureurSale_Insert",
                p => new
                    {
                        CompteurFacture = p.Int(),
                        SaleDeliver = p.Boolean(),
                        VatRate = p.Double(),
                        RateReduction = p.Double(),
                        RateDiscount = p.Double(),
                        Transport = p.Double(),
                        SaleDeliveryDate = p.DateTime(),
                        SaleDate = p.DateTime(),
                        SaleDateHours = p.DateTime(),
                        SaleValidate = p.Boolean(),
                        PoliceAssurance = p.String(),
                        PaymentDelay = p.Int(),
                        Guaranteed = p.Int(),
                        Patient = p.String(),
                        DeviseID = p.Int(),
                        IsPaid = p.Boolean(),
                        SaleReceiptNumber = p.String(maxLength: 100),
                        BranchID = p.Int(),
                        CustomerID = p.Int(),
                        CustomerName = p.String(),
                        isReturn = p.Boolean(),
                        StatutSale = p.Int(),
                        OperatorID = p.Int(),
                        PostByID = p.Int(),
                        IsSpecialOrder = p.Boolean(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        CustomerOrderID = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        DateRdv = p.DateTime(),
                        IsRendezVous = p.Boolean(),
                        AwaitingDay = p.Int(),
                        IsProductReveive = p.Boolean(),
                        EffectiveReceiveDate = p.DateTime(),
                        IsCustomerRceive = p.Boolean(),
                        CustomerDeliverDate = p.DateTime(),
                        PostSOByID = p.Int(),
                        ReceiveSOByID = p.Int(),
                        CNI = p.String(maxLength: 100),
                        AssureurPMID = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [CNI], [Consultation_ConsultationID])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @CNI, @Consultation_ConsultationID)
                      
                      DECLARE @SaleID int
                      SELECT @SaleID = [SaleID]
                      FROM [dbo].[Sales]
                      WHERE @@ROWCOUNT > 0 AND [SaleID] = scope_identity()
                      
                      INSERT [dbo].[AssureurSales]([SaleID], [AssureurPMID])
                      VALUES (@SaleID, @AssureurPMID)
                      
                      SELECT t0.[SaleID]
                      FROM [dbo].[Sales] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[SaleID] = @SaleID"
            );
            
            AlterStoredProcedure(
                "dbo.AssureurSale_Update",
                p => new
                    {
                        SaleID = p.Int(),
                        CompteurFacture = p.Int(),
                        SaleDeliver = p.Boolean(),
                        VatRate = p.Double(),
                        RateReduction = p.Double(),
                        RateDiscount = p.Double(),
                        Transport = p.Double(),
                        SaleDeliveryDate = p.DateTime(),
                        SaleDate = p.DateTime(),
                        SaleDateHours = p.DateTime(),
                        SaleValidate = p.Boolean(),
                        PoliceAssurance = p.String(),
                        PaymentDelay = p.Int(),
                        Guaranteed = p.Int(),
                        Patient = p.String(),
                        DeviseID = p.Int(),
                        IsPaid = p.Boolean(),
                        SaleReceiptNumber = p.String(maxLength: 100),
                        BranchID = p.Int(),
                        CustomerID = p.Int(),
                        CustomerName = p.String(),
                        isReturn = p.Boolean(),
                        StatutSale = p.Int(),
                        OperatorID = p.Int(),
                        PostByID = p.Int(),
                        IsSpecialOrder = p.Boolean(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        CustomerOrderID = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        DateRdv = p.DateTime(),
                        IsRendezVous = p.Boolean(),
                        AwaitingDay = p.Int(),
                        IsProductReveive = p.Boolean(),
                        EffectiveReceiveDate = p.DateTime(),
                        IsCustomerRceive = p.Boolean(),
                        CustomerDeliverDate = p.DateTime(),
                        PostSOByID = p.Int(),
                        ReceiveSOByID = p.Int(),
                        CNI = p.String(maxLength: 100),
                        AssureurPMID = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[AssureurSales]
                      SET [AssureurPMID] = @AssureurPMID
                      WHERE ([SaleID] = @SaleID)
                      
                      UPDATE [dbo].[Sales]
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [CNI] = @CNI, [Consultation_ConsultationID] = @Consultation_ConsultationID
                      WHERE ([SaleID] = @SaleID)
                      AND @@ROWCOUNT > 0"
            );
            
            AlterStoredProcedure(
                "dbo.AssureurSale_Delete",
                p => new
                    {
                        SaleID = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[AssureurSales]
                      WHERE ([SaleID] = @SaleID)
                      
                      DELETE [dbo].[Sales]
                      WHERE (([SaleID] = @SaleID) AND (([Consultation_ConsultationID] = @Consultation_ConsultationID) OR ([Consultation_ConsultationID] IS NULL AND @Consultation_ConsultationID IS NULL)))
                      AND @@ROWCOUNT > 0"
            );
            
            AlterStoredProcedure(
                "dbo.SavingAccountSale_Insert",
                p => new
                    {
                        CompteurFacture = p.Int(),
                        SaleDeliver = p.Boolean(),
                        VatRate = p.Double(),
                        RateReduction = p.Double(),
                        RateDiscount = p.Double(),
                        Transport = p.Double(),
                        SaleDeliveryDate = p.DateTime(),
                        SaleDate = p.DateTime(),
                        SaleDateHours = p.DateTime(),
                        SaleValidate = p.Boolean(),
                        PoliceAssurance = p.String(),
                        PaymentDelay = p.Int(),
                        Guaranteed = p.Int(),
                        Patient = p.String(),
                        DeviseID = p.Int(),
                        IsPaid = p.Boolean(),
                        SaleReceiptNumber = p.String(maxLength: 100),
                        BranchID = p.Int(),
                        CustomerID = p.Int(),
                        CustomerName = p.String(),
                        isReturn = p.Boolean(),
                        StatutSale = p.Int(),
                        OperatorID = p.Int(),
                        PostByID = p.Int(),
                        IsSpecialOrder = p.Boolean(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        CustomerOrderID = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        DateRdv = p.DateTime(),
                        IsRendezVous = p.Boolean(),
                        AwaitingDay = p.Int(),
                        IsProductReveive = p.Boolean(),
                        EffectiveReceiveDate = p.DateTime(),
                        IsCustomerRceive = p.Boolean(),
                        CustomerDeliverDate = p.DateTime(),
                        PostSOByID = p.Int(),
                        ReceiveSOByID = p.Int(),
                        CNI = p.String(maxLength: 100),
                        SavingAccountID = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [CNI], [Consultation_ConsultationID])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @CNI, @Consultation_ConsultationID)
                      
                      DECLARE @SaleID int
                      SELECT @SaleID = [SaleID]
                      FROM [dbo].[Sales]
                      WHERE @@ROWCOUNT > 0 AND [SaleID] = scope_identity()
                      
                      INSERT [dbo].[SavingAccountSales]([SaleID], [SavingAccountID])
                      VALUES (@SaleID, @SavingAccountID)
                      
                      SELECT t0.[SaleID]
                      FROM [dbo].[Sales] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[SaleID] = @SaleID"
            );
            
            AlterStoredProcedure(
                "dbo.SavingAccountSale_Update",
                p => new
                    {
                        SaleID = p.Int(),
                        CompteurFacture = p.Int(),
                        SaleDeliver = p.Boolean(),
                        VatRate = p.Double(),
                        RateReduction = p.Double(),
                        RateDiscount = p.Double(),
                        Transport = p.Double(),
                        SaleDeliveryDate = p.DateTime(),
                        SaleDate = p.DateTime(),
                        SaleDateHours = p.DateTime(),
                        SaleValidate = p.Boolean(),
                        PoliceAssurance = p.String(),
                        PaymentDelay = p.Int(),
                        Guaranteed = p.Int(),
                        Patient = p.String(),
                        DeviseID = p.Int(),
                        IsPaid = p.Boolean(),
                        SaleReceiptNumber = p.String(maxLength: 100),
                        BranchID = p.Int(),
                        CustomerID = p.Int(),
                        CustomerName = p.String(),
                        isReturn = p.Boolean(),
                        StatutSale = p.Int(),
                        OperatorID = p.Int(),
                        PostByID = p.Int(),
                        IsSpecialOrder = p.Boolean(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        CustomerOrderID = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        DateRdv = p.DateTime(),
                        IsRendezVous = p.Boolean(),
                        AwaitingDay = p.Int(),
                        IsProductReveive = p.Boolean(),
                        EffectiveReceiveDate = p.DateTime(),
                        IsCustomerRceive = p.Boolean(),
                        CustomerDeliverDate = p.DateTime(),
                        PostSOByID = p.Int(),
                        ReceiveSOByID = p.Int(),
                        CNI = p.String(maxLength: 100),
                        SavingAccountID = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Sales]
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [CNI] = @CNI, [Consultation_ConsultationID] = @Consultation_ConsultationID
                      WHERE ([SaleID] = @SaleID)
                      
                      UPDATE [dbo].[SavingAccountSales]
                      SET [SavingAccountID] = @SavingAccountID
                      WHERE ([SaleID] = @SaleID)
                      AND @@ROWCOUNT > 0"
            );
            
            AlterStoredProcedure(
                "dbo.SavingAccountSale_Delete",
                p => new
                    {
                        SaleID = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[SavingAccountSales]
                      WHERE ([SaleID] = @SaleID)
                      
                      DELETE [dbo].[Sales]
                      WHERE (([SaleID] = @SaleID) AND (([Consultation_ConsultationID] = @Consultation_ConsultationID) OR ([Consultation_ConsultationID] IS NULL AND @Consultation_ConsultationID IS NULL)))
                      AND @@ROWCOUNT > 0"
            );
            
        }
        
        public override void Down()
        {
            DropStoredProcedure("dbo.Consultation_Delete");
            DropStoredProcedure("dbo.Consultation_Update");
            DropStoredProcedure("dbo.Consultation_Insert");
            AddColumn("dbo.Prescriptions", "CustomerID", c => c.Int(nullable: false));
            AddColumn("dbo.Customers", "isPrescritionValidate", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.Prescriptions", "ConsultationID", "dbo.Consultations");
            DropForeignKey("dbo.Sales", "Consultation_ConsultationID", "dbo.Consultations");
            DropForeignKey("dbo.Consultations", "CustomerID", "dbo.Customers");
            DropIndex("dbo.Prescriptions", new[] { "ConsultationID" });
            DropIndex("dbo.Consultations", new[] { "CustomerID" });
            DropIndex("dbo.Sales", new[] { "Consultation_ConsultationID" });
            DropColumn("dbo.Prescriptions", "ConsultationID");
            DropColumn("dbo.Sales", "Consultation_ConsultationID");
            DropTable("dbo.Consultations");
            CreateIndex("dbo.Prescriptions", "CustomerID");
            AddForeignKey("dbo.Prescriptions", "CustomerID", "dbo.Customers", "GlobalPersonID");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
