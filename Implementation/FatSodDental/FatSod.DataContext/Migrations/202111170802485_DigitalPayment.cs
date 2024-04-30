namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DigitalPayment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DigitalPaymentMethods",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        AccountID = c.Int(nullable: false),
                        AccountManagerId = c.Int(nullable: false),
                        IsEnable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.PaymentMethods", t => t.ID)
                .ForeignKey("dbo.Accounts", t => t.AccountID)
                .ForeignKey("dbo.Users", t => t.AccountManagerId)
                .Index(t => t.ID)
                .Index(t => t.AccountID)
                .Index(t => t.AccountManagerId);
            
            CreateTable(
                "dbo.DigitalPaymentSales",
                c => new
                    {
                        SaleID = c.Int(nullable: false),
                        TransactionIdentifier = c.String(maxLength: 50),
                        DigitalPaymentMethodId = c.Int(nullable: false),
                        DigitalAccountManagerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SaleID)
                .ForeignKey("dbo.Sales", t => t.SaleID)
                .ForeignKey("dbo.DigitalPaymentMethods", t => t.DigitalPaymentMethodId)
                .ForeignKey("dbo.Users", t => t.DigitalAccountManagerId)
                .Index(t => t.SaleID)
                .Index(t => new { t.TransactionIdentifier, t.DigitalPaymentMethodId }, unique: true, name: "IX_RealPrimaryKey")
                .Index(t => t.DigitalAccountManagerId);
            
            CreateStoredProcedure(
                "dbo.DigitalPaymentSale_Insert",
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
                        TransactionIdentifier = p.String(maxLength: 50),
                        DigitalPaymentMethodId = p.Int(),
                        DigitalAccountManagerId = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [SellerID], [ConsultDilPrescID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [GestionnaireID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [IsUrgent], [CNI], [Consultation_ConsultationID])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @SellerID, @ConsultDilPrescID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @GestionnaireID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @IsUrgent, @CNI, @Consultation_ConsultationID)
                      
                      DECLARE @SaleID int
                      SELECT @SaleID = [SaleID]
                      FROM [dbo].[Sales]
                      WHERE @@ROWCOUNT > 0 AND [SaleID] = scope_identity()
                      
                      INSERT [dbo].[DigitalPaymentSales]([SaleID], [TransactionIdentifier], [DigitalPaymentMethodId], [DigitalAccountManagerId])
                      VALUES (@SaleID, @TransactionIdentifier, @DigitalPaymentMethodId, @DigitalAccountManagerId)
                      
                      SELECT t0.[SaleID]
                      FROM [dbo].[Sales] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[SaleID] = @SaleID"
            );
            
            CreateStoredProcedure(
                "dbo.DigitalPaymentSale_Update",
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
                        TransactionIdentifier = p.String(maxLength: 50),
                        DigitalPaymentMethodId = p.Int(),
                        DigitalAccountManagerId = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[DigitalPaymentSales]
                      SET [TransactionIdentifier] = @TransactionIdentifier, [DigitalPaymentMethodId] = @DigitalPaymentMethodId, [DigitalAccountManagerId] = @DigitalAccountManagerId
                      WHERE ([SaleID] = @SaleID)
                      
                      UPDATE [dbo].[Sales]
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [SellerID] = @SellerID, [ConsultDilPrescID] = @ConsultDilPrescID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [GestionnaireID] = @GestionnaireID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [IsUrgent] = @IsUrgent, [CNI] = @CNI, [Consultation_ConsultationID] = @Consultation_ConsultationID
                      WHERE ([SaleID] = @SaleID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.DigitalPaymentSale_Delete",
                p => new
                    {
                        SaleID = p.Int(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[DigitalPaymentSales]
                      WHERE ([SaleID] = @SaleID)
                      
                      DELETE [dbo].[Sales]
                      WHERE (([SaleID] = @SaleID) AND (([Consultation_ConsultationID] = @Consultation_ConsultationID) OR ([Consultation_ConsultationID] IS NULL AND @Consultation_ConsultationID IS NULL)))
                      AND @@ROWCOUNT > 0"
            );
            
        }
        
        public override void Down()
        {
            DropStoredProcedure("dbo.DigitalPaymentSale_Delete");
            DropStoredProcedure("dbo.DigitalPaymentSale_Update");
            DropStoredProcedure("dbo.DigitalPaymentSale_Insert");
            DropForeignKey("dbo.DigitalPaymentSales", "DigitalAccountManagerId", "dbo.Users");
            DropForeignKey("dbo.DigitalPaymentSales", "DigitalPaymentMethodId", "dbo.DigitalPaymentMethods");
            DropForeignKey("dbo.DigitalPaymentSales", "SaleID", "dbo.Sales");
            DropForeignKey("dbo.DigitalPaymentMethods", "AccountManagerId", "dbo.Users");
            DropForeignKey("dbo.DigitalPaymentMethods", "AccountID", "dbo.Accounts");
            DropForeignKey("dbo.DigitalPaymentMethods", "ID", "dbo.PaymentMethods");
            DropIndex("dbo.DigitalPaymentSales", new[] { "DigitalAccountManagerId" });
            DropIndex("dbo.DigitalPaymentSales", "IX_RealPrimaryKey");
            DropIndex("dbo.DigitalPaymentSales", new[] { "SaleID" });
            DropIndex("dbo.DigitalPaymentMethods", new[] { "AccountManagerId" });
            DropIndex("dbo.DigitalPaymentMethods", new[] { "AccountID" });
            DropIndex("dbo.DigitalPaymentMethods", new[] { "ID" });
            DropTable("dbo.DigitalPaymentSales");
            DropTable("dbo.DigitalPaymentMethods");
        }
    }
}
