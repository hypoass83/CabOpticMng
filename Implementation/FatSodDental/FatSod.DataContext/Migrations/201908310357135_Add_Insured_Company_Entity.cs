namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Insured_Company_Entity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InsuredCompanies",
                c => new
                    {
                        InsuredCompanyID = c.Int(nullable: false, identity: true),
                        InsuredCompanyCode = c.String(nullable: false, maxLength: 100),
                        InsuredCompanyLabel = c.String(),
                    })
                .PrimaryKey(t => t.InsuredCompanyID)
                .Index(t => t.InsuredCompanyCode, unique: true, name: "XI_InsuredCompany");
            
            AddColumn("dbo.CustomerOrders", "InsuredCompanyID", c => c.Int());
            CreateIndex("dbo.CustomerOrders", "InsuredCompanyID");
            AddForeignKey("dbo.CustomerOrders", "InsuredCompanyID", "dbo.InsuredCompanies", "InsuredCompanyID");
            AlterStoredProcedure(
                "dbo.CustomerOrder_Insert",
                p => new
                    {
                        CompteurFacture = p.Int(),
                        CustomerOrderDate = p.DateTime(),
                        CustomerDateHours = p.DateTime(),
                        VatRate = p.Double(),
                        RateReduction = p.Double(),
                        RateDiscount = p.Double(),
                        Patient = p.String(),
                        CustomerOrderNumber = p.String(maxLength: 100),
                        IsDelivered = p.Boolean(),
                        CustomerName = p.String(),
                        InsurreName = p.String(),
                        PhoneNumber = p.String(),
                        PoliceAssurance = p.String(maxLength: 250),
                        CompanyName = p.String(maxLength: 250),
                        AssureurID = p.Int(),
                        LieuxdeDepotBorderoID = p.Int(),
                        InsuredCompanyID = p.Int(),
                        DeviseID = p.Int(),
                        BranchID = p.Int(),
                        OperatorID = p.Int(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        Transport = p.Double(),
                        PlafondAssurance = p.Double(),
                        NumeroBonPriseEnCharge = p.String(),
                        VerreAssurance = p.Double(),
                        MontureAssurance = p.Double(),
                        Plafond = p.Double(),
                        TotalMalade = p.Double(),
                        NumeroFacture = p.String(maxLength: 100),
                        BillState = p.Int(),
                        DatailBill = p.Int(),
                        MntValidate = p.Double(),
                        GestionnaireID = p.Int(),
                        ValidateBillDate = p.DateTime(),
                        BorderoDepotID = p.Int(),
                        DeleteReason = p.String(),
                        DeleteBillDate = p.DateTime(),
                        DeletedByID = p.Int(),
                        MntValideBordero = p.Double(),
                        isMntValideBordero = p.Boolean(),
                    },
                body:
                    @"INSERT [dbo].[CustomerOrders]([CompteurFacture], [CustomerOrderDate], [CustomerDateHours], [VatRate], [RateReduction], [RateDiscount], [Patient], [CustomerOrderNumber], [IsDelivered], [CustomerName], [InsurreName], [PhoneNumber], [PoliceAssurance], [CompanyName], [AssureurID], [LieuxdeDepotBorderoID], [InsuredCompanyID], [DeviseID], [BranchID], [OperatorID], [Remarque], [MedecinTraitant], [Transport], [PlafondAssurance], [NumeroBonPriseEnCharge], [VerreAssurance], [MontureAssurance], [Plafond], [TotalMalade], [NumeroFacture], [BillState], [DatailBill], [MntValidate], [GestionnaireID], [ValidateBillDate], [BorderoDepotID], [DeleteReason], [DeleteBillDate], [DeletedByID], [MntValideBordero], [isMntValideBordero])
                      VALUES (@CompteurFacture, @CustomerOrderDate, @CustomerDateHours, @VatRate, @RateReduction, @RateDiscount, @Patient, @CustomerOrderNumber, @IsDelivered, @CustomerName, @InsurreName, @PhoneNumber, @PoliceAssurance, @CompanyName, @AssureurID, @LieuxdeDepotBorderoID, @InsuredCompanyID, @DeviseID, @BranchID, @OperatorID, @Remarque, @MedecinTraitant, @Transport, @PlafondAssurance, @NumeroBonPriseEnCharge, @VerreAssurance, @MontureAssurance, @Plafond, @TotalMalade, @NumeroFacture, @BillState, @DatailBill, @MntValidate, @GestionnaireID, @ValidateBillDate, @BorderoDepotID, @DeleteReason, @DeleteBillDate, @DeletedByID, @MntValideBordero, @isMntValideBordero)
                      
                      DECLARE @CustomerOrderID int
                      SELECT @CustomerOrderID = [CustomerOrderID]
                      FROM [dbo].[CustomerOrders]
                      WHERE @@ROWCOUNT > 0 AND [CustomerOrderID] = scope_identity()
                      
                      SELECT t0.[CustomerOrderID]
                      FROM [dbo].[CustomerOrders] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[CustomerOrderID] = @CustomerOrderID"
            );
            
            AlterStoredProcedure(
                "dbo.CustomerOrder_Update",
                p => new
                    {
                        CustomerOrderID = p.Int(),
                        CompteurFacture = p.Int(),
                        CustomerOrderDate = p.DateTime(),
                        CustomerDateHours = p.DateTime(),
                        VatRate = p.Double(),
                        RateReduction = p.Double(),
                        RateDiscount = p.Double(),
                        Patient = p.String(),
                        CustomerOrderNumber = p.String(maxLength: 100),
                        IsDelivered = p.Boolean(),
                        CustomerName = p.String(),
                        InsurreName = p.String(),
                        PhoneNumber = p.String(),
                        PoliceAssurance = p.String(maxLength: 250),
                        CompanyName = p.String(maxLength: 250),
                        AssureurID = p.Int(),
                        LieuxdeDepotBorderoID = p.Int(),
                        InsuredCompanyID = p.Int(),
                        DeviseID = p.Int(),
                        BranchID = p.Int(),
                        OperatorID = p.Int(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        Transport = p.Double(),
                        PlafondAssurance = p.Double(),
                        NumeroBonPriseEnCharge = p.String(),
                        VerreAssurance = p.Double(),
                        MontureAssurance = p.Double(),
                        Plafond = p.Double(),
                        TotalMalade = p.Double(),
                        NumeroFacture = p.String(maxLength: 100),
                        BillState = p.Int(),
                        DatailBill = p.Int(),
                        MntValidate = p.Double(),
                        GestionnaireID = p.Int(),
                        ValidateBillDate = p.DateTime(),
                        BorderoDepotID = p.Int(),
                        DeleteReason = p.String(),
                        DeleteBillDate = p.DateTime(),
                        DeletedByID = p.Int(),
                        MntValideBordero = p.Double(),
                        isMntValideBordero = p.Boolean(),
                    },
                body:
                    @"UPDATE [dbo].[CustomerOrders]
                      SET [CompteurFacture] = @CompteurFacture, [CustomerOrderDate] = @CustomerOrderDate, [CustomerDateHours] = @CustomerDateHours, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Patient] = @Patient, [CustomerOrderNumber] = @CustomerOrderNumber, [IsDelivered] = @IsDelivered, [CustomerName] = @CustomerName, [InsurreName] = @InsurreName, [PhoneNumber] = @PhoneNumber, [PoliceAssurance] = @PoliceAssurance, [CompanyName] = @CompanyName, [AssureurID] = @AssureurID, [LieuxdeDepotBorderoID] = @LieuxdeDepotBorderoID, [InsuredCompanyID] = @InsuredCompanyID, [DeviseID] = @DeviseID, [BranchID] = @BranchID, [OperatorID] = @OperatorID, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [Transport] = @Transport, [PlafondAssurance] = @PlafondAssurance, [NumeroBonPriseEnCharge] = @NumeroBonPriseEnCharge, [VerreAssurance] = @VerreAssurance, [MontureAssurance] = @MontureAssurance, [Plafond] = @Plafond, [TotalMalade] = @TotalMalade, [NumeroFacture] = @NumeroFacture, [BillState] = @BillState, [DatailBill] = @DatailBill, [MntValidate] = @MntValidate, [GestionnaireID] = @GestionnaireID, [ValidateBillDate] = @ValidateBillDate, [BorderoDepotID] = @BorderoDepotID, [DeleteReason] = @DeleteReason, [DeleteBillDate] = @DeleteBillDate, [DeletedByID] = @DeletedByID, [MntValideBordero] = @MntValideBordero, [isMntValideBordero] = @isMntValideBordero
                      WHERE ([CustomerOrderID] = @CustomerOrderID)"
            );
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerOrders", "InsuredCompanyID", "dbo.InsuredCompanies");
            DropIndex("dbo.InsuredCompanies", "XI_InsuredCompany");
            DropIndex("dbo.CustomerOrders", new[] { "InsuredCompanyID" });
            DropColumn("dbo.CustomerOrders", "InsuredCompanyID");
            DropTable("dbo.InsuredCompanies");
            //throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
