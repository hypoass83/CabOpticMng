namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_IsUrgent_To_Sale : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "IsUrgent", c => c.Boolean(nullable: false));
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
                        SellerID = p.Int(),
                        ConsultDilPrescID = p.Int(),
                        IsSpecialOrder = p.Boolean(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        CustomerOrderID = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        GestionnaireID = p.Int(),
                        DateRdv = p.DateTime(),
                        IsRendezVous = p.Boolean(),
                        AwaitingDay = p.Int(),
                        IsProductReveive = p.Boolean(),
                        EffectiveReceiveDate = p.DateTime(),
                        IsCustomerRceive = p.Boolean(),
                        CustomerDeliverDate = p.DateTime(),
                        PostSOByID = p.Int(),
                        ReceiveSOByID = p.Int(),
                        IsUrgent = p.Boolean(),
                        CNI = p.String(maxLength: 100),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [SellerID], [ConsultDilPrescID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [GestionnaireID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [IsUrgent], [CNI], [Consultation_ConsultationID])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @SellerID, @ConsultDilPrescID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @GestionnaireID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @IsUrgent, @CNI, @Consultation_ConsultationID)
                      
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
                        SellerID = p.Int(),
                        ConsultDilPrescID = p.Int(),
                        IsSpecialOrder = p.Boolean(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        CustomerOrderID = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        GestionnaireID = p.Int(),
                        DateRdv = p.DateTime(),
                        IsRendezVous = p.Boolean(),
                        AwaitingDay = p.Int(),
                        IsProductReveive = p.Boolean(),
                        EffectiveReceiveDate = p.DateTime(),
                        IsCustomerRceive = p.Boolean(),
                        CustomerDeliverDate = p.DateTime(),
                        PostSOByID = p.Int(),
                        ReceiveSOByID = p.Int(),
                        IsUrgent = p.Boolean(),
                        CNI = p.String(maxLength: 100),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Sales]
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [SellerID] = @SellerID, [ConsultDilPrescID] = @ConsultDilPrescID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [GestionnaireID] = @GestionnaireID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [IsUrgent] = @IsUrgent, [CNI] = @CNI, [Consultation_ConsultationID] = @Consultation_ConsultationID
                      WHERE ([SaleID] = @SaleID)"
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
                        SellerID = p.Int(),
                        ConsultDilPrescID = p.Int(),
                        IsSpecialOrder = p.Boolean(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        CustomerOrderID = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        GestionnaireID = p.Int(),
                        DateRdv = p.DateTime(),
                        IsRendezVous = p.Boolean(),
                        AwaitingDay = p.Int(),
                        IsProductReveive = p.Boolean(),
                        EffectiveReceiveDate = p.DateTime(),
                        IsCustomerRceive = p.Boolean(),
                        CustomerDeliverDate = p.DateTime(),
                        PostSOByID = p.Int(),
                        ReceiveSOByID = p.Int(),
                        IsUrgent = p.Boolean(),
                        CNI = p.String(maxLength: 100),
                        AssureurPMID = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [SellerID], [ConsultDilPrescID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [GestionnaireID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [IsUrgent], [CNI], [Consultation_ConsultationID])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @SellerID, @ConsultDilPrescID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @GestionnaireID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @IsUrgent, @CNI, @Consultation_ConsultationID)
                      
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
                        SellerID = p.Int(),
                        ConsultDilPrescID = p.Int(),
                        IsSpecialOrder = p.Boolean(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        CustomerOrderID = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        GestionnaireID = p.Int(),
                        DateRdv = p.DateTime(),
                        IsRendezVous = p.Boolean(),
                        AwaitingDay = p.Int(),
                        IsProductReveive = p.Boolean(),
                        EffectiveReceiveDate = p.DateTime(),
                        IsCustomerRceive = p.Boolean(),
                        CustomerDeliverDate = p.DateTime(),
                        PostSOByID = p.Int(),
                        ReceiveSOByID = p.Int(),
                        IsUrgent = p.Boolean(),
                        CNI = p.String(maxLength: 100),
                        AssureurPMID = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[AssureurSales]
                      SET [AssureurPMID] = @AssureurPMID
                      WHERE ([SaleID] = @SaleID)
                      
                      UPDATE [dbo].[Sales]
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [SellerID] = @SellerID, [ConsultDilPrescID] = @ConsultDilPrescID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [GestionnaireID] = @GestionnaireID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [IsUrgent] = @IsUrgent, [CNI] = @CNI, [Consultation_ConsultationID] = @Consultation_ConsultationID
                      WHERE ([SaleID] = @SaleID)
                      AND @@ROWCOUNT > 0"
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
                        SellerID = p.Int(),
                        ConsultDilPrescID = p.Int(),
                        IsSpecialOrder = p.Boolean(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        CustomerOrderID = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        GestionnaireID = p.Int(),
                        DateRdv = p.DateTime(),
                        IsRendezVous = p.Boolean(),
                        AwaitingDay = p.Int(),
                        IsProductReveive = p.Boolean(),
                        EffectiveReceiveDate = p.DateTime(),
                        IsCustomerRceive = p.Boolean(),
                        CustomerDeliverDate = p.DateTime(),
                        PostSOByID = p.Int(),
                        ReceiveSOByID = p.Int(),
                        IsUrgent = p.Boolean(),
                        CNI = p.String(maxLength: 100),
                        BankID = p.Int(),
                        BankRef = p.String(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [SellerID], [ConsultDilPrescID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [GestionnaireID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [IsUrgent], [CNI], [Consultation_ConsultationID])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @SellerID, @ConsultDilPrescID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @GestionnaireID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @IsUrgent, @CNI, @Consultation_ConsultationID)
                      
                      DECLARE @SaleID int
                      SELECT @SaleID = [SaleID]
                      FROM [dbo].[Sales]
                      WHERE @@ROWCOUNT > 0 AND [SaleID] = scope_identity()
                      
                      INSERT [dbo].[BankSales]([SaleID], [BankID], [BankRef])
                      VALUES (@SaleID, @BankID, @BankRef)
                      
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
                        SellerID = p.Int(),
                        ConsultDilPrescID = p.Int(),
                        IsSpecialOrder = p.Boolean(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        CustomerOrderID = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        GestionnaireID = p.Int(),
                        DateRdv = p.DateTime(),
                        IsRendezVous = p.Boolean(),
                        AwaitingDay = p.Int(),
                        IsProductReveive = p.Boolean(),
                        EffectiveReceiveDate = p.DateTime(),
                        IsCustomerRceive = p.Boolean(),
                        CustomerDeliverDate = p.DateTime(),
                        PostSOByID = p.Int(),
                        ReceiveSOByID = p.Int(),
                        IsUrgent = p.Boolean(),
                        CNI = p.String(maxLength: 100),
                        BankID = p.Int(),
                        BankRef = p.String(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[BankSales]
                      SET [BankID] = @BankID, [BankRef] = @BankRef
                      WHERE ([SaleID] = @SaleID)
                      
                      UPDATE [dbo].[Sales]
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [SellerID] = @SellerID, [ConsultDilPrescID] = @ConsultDilPrescID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [GestionnaireID] = @GestionnaireID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [IsUrgent] = @IsUrgent, [CNI] = @CNI, [Consultation_ConsultationID] = @Consultation_ConsultationID
                      WHERE ([SaleID] = @SaleID)
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
                        SellerID = p.Int(),
                        ConsultDilPrescID = p.Int(),
                        IsSpecialOrder = p.Boolean(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        CustomerOrderID = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        GestionnaireID = p.Int(),
                        DateRdv = p.DateTime(),
                        IsRendezVous = p.Boolean(),
                        AwaitingDay = p.Int(),
                        IsProductReveive = p.Boolean(),
                        EffectiveReceiveDate = p.DateTime(),
                        IsCustomerRceive = p.Boolean(),
                        CustomerDeliverDate = p.DateTime(),
                        PostSOByID = p.Int(),
                        ReceiveSOByID = p.Int(),
                        IsUrgent = p.Boolean(),
                        CNI = p.String(maxLength: 100),
                        SavingAccountID = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [SellerID], [ConsultDilPrescID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [GestionnaireID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [IsUrgent], [CNI], [Consultation_ConsultationID])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @SellerID, @ConsultDilPrescID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @GestionnaireID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @IsUrgent, @CNI, @Consultation_ConsultationID)
                      
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
                        SellerID = p.Int(),
                        ConsultDilPrescID = p.Int(),
                        IsSpecialOrder = p.Boolean(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        CustomerOrderID = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        GestionnaireID = p.Int(),
                        DateRdv = p.DateTime(),
                        IsRendezVous = p.Boolean(),
                        AwaitingDay = p.Int(),
                        IsProductReveive = p.Boolean(),
                        EffectiveReceiveDate = p.DateTime(),
                        IsCustomerRceive = p.Boolean(),
                        CustomerDeliverDate = p.DateTime(),
                        PostSOByID = p.Int(),
                        ReceiveSOByID = p.Int(),
                        IsUrgent = p.Boolean(),
                        CNI = p.String(maxLength: 100),
                        SavingAccountID = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Sales]
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [SellerID] = @SellerID, [ConsultDilPrescID] = @ConsultDilPrescID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [GestionnaireID] = @GestionnaireID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [IsUrgent] = @IsUrgent, [CNI] = @CNI, [Consultation_ConsultationID] = @Consultation_ConsultationID
                      WHERE ([SaleID] = @SaleID)
                      
                      UPDATE [dbo].[SavingAccountSales]
                      SET [SavingAccountID] = @SavingAccountID
                      WHERE ([SaleID] = @SaleID)
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
                        SellerID = p.Int(),
                        ConsultDilPrescID = p.Int(),
                        IsSpecialOrder = p.Boolean(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        CustomerOrderID = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        GestionnaireID = p.Int(),
                        DateRdv = p.DateTime(),
                        IsRendezVous = p.Boolean(),
                        AwaitingDay = p.Int(),
                        IsProductReveive = p.Boolean(),
                        EffectiveReceiveDate = p.DateTime(),
                        IsCustomerRceive = p.Boolean(),
                        CustomerDeliverDate = p.DateTime(),
                        PostSOByID = p.Int(),
                        ReceiveSOByID = p.Int(),
                        IsUrgent = p.Boolean(),
                        CNI = p.String(maxLength: 100),
                        TillID = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [SellerID], [ConsultDilPrescID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [GestionnaireID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [IsUrgent], [CNI], [Consultation_ConsultationID])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @SellerID, @ConsultDilPrescID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @GestionnaireID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @IsUrgent, @CNI, @Consultation_ConsultationID)
                      
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
                        SellerID = p.Int(),
                        ConsultDilPrescID = p.Int(),
                        IsSpecialOrder = p.Boolean(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        CustomerOrderID = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        GestionnaireID = p.Int(),
                        DateRdv = p.DateTime(),
                        IsRendezVous = p.Boolean(),
                        AwaitingDay = p.Int(),
                        IsProductReveive = p.Boolean(),
                        EffectiveReceiveDate = p.DateTime(),
                        IsCustomerRceive = p.Boolean(),
                        CustomerDeliverDate = p.DateTime(),
                        PostSOByID = p.Int(),
                        ReceiveSOByID = p.Int(),
                        IsUrgent = p.Boolean(),
                        CNI = p.String(maxLength: 100),
                        TillID = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Sales]
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [SellerID] = @SellerID, [ConsultDilPrescID] = @ConsultDilPrescID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [GestionnaireID] = @GestionnaireID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [IsUrgent] = @IsUrgent, [CNI] = @CNI, [Consultation_ConsultationID] = @Consultation_ConsultationID
                      WHERE ([SaleID] = @SaleID)
                      
                      UPDATE [dbo].[TillSales]
                      SET [TillID] = @TillID
                      WHERE ([SaleID] = @SaleID)
                      AND @@ROWCOUNT > 0"
            );
            
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sales", "IsUrgent");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
