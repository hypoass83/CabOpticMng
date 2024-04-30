namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InsureBill_add_patient_and_LieuxdeDepot_til_Presc : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LieuxdeDepotBorderoes",
                c => new
                    {
                        LieuxdeDepotBorderoID = c.Int(nullable: false, identity: true),
                        LieuxdeDepotBorderoName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.LieuxdeDepotBorderoID)
                .Index(t => t.LieuxdeDepotBorderoName, unique: true, name: "IX_LieuxdeDepotBordero");
            
            AddColumn("dbo.AuthoriseSales", "IsDilatation", c => c.Boolean(nullable: false));
            AddColumn("dbo.CustomerOrders", "InsurreName", c => c.String(nullable: false));
            AddColumn("dbo.CustomerOrders", "LieuxdeDepotBorderoID", c => c.Int());
            AddColumn("dbo.Prescriptions", "isAuthoriseSale", c => c.Boolean(nullable: false));
            AddColumn("dbo.TillDays", "TillDayCashHand", c => c.Double(nullable: false));
            CreateIndex("dbo.CustomerOrders", "LieuxdeDepotBorderoID");
            AddForeignKey("dbo.CustomerOrders", "LieuxdeDepotBorderoID", "dbo.LieuxdeDepotBorderoes", "LieuxdeDepotBorderoID");
            DropColumn("dbo.Prescriptions", "isPrescriptionValidate");
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
                    },
                body:
                    @"INSERT [dbo].[CustomerOrders]([CompteurFacture], [CustomerOrderDate], [CustomerDateHours], [VatRate], [RateReduction], [RateDiscount], [Patient], [CustomerOrderNumber], [IsDelivered], [CustomerName], [InsurreName], [PhoneNumber], [PoliceAssurance], [CompanyName], [AssureurID], [LieuxdeDepotBorderoID], [DeviseID], [BranchID], [OperatorID], [Remarque], [MedecinTraitant], [Transport], [PlafondAssurance], [NumeroBonPriseEnCharge], [VerreAssurance], [MontureAssurance], [Plafond], [TotalMalade], [NumeroFacture], [BillState], [DatailBill], [MntValidate], [GestionnaireID], [ValidateBillDate], [BorderoDepotID], [DeleteReason], [DeleteBillDate], [DeletedByID])
                      VALUES (@CompteurFacture, @CustomerOrderDate, @CustomerDateHours, @VatRate, @RateReduction, @RateDiscount, @Patient, @CustomerOrderNumber, @IsDelivered, @CustomerName, @InsurreName, @PhoneNumber, @PoliceAssurance, @CompanyName, @AssureurID, @LieuxdeDepotBorderoID, @DeviseID, @BranchID, @OperatorID, @Remarque, @MedecinTraitant, @Transport, @PlafondAssurance, @NumeroBonPriseEnCharge, @VerreAssurance, @MontureAssurance, @Plafond, @TotalMalade, @NumeroFacture, @BillState, @DatailBill, @MntValidate, @GestionnaireID, @ValidateBillDate, @BorderoDepotID, @DeleteReason, @DeleteBillDate, @DeletedByID)
                      
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
                    },
                body:
                    @"UPDATE [dbo].[CustomerOrders]
                      SET [CompteurFacture] = @CompteurFacture, [CustomerOrderDate] = @CustomerOrderDate, [CustomerDateHours] = @CustomerDateHours, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Patient] = @Patient, [CustomerOrderNumber] = @CustomerOrderNumber, [IsDelivered] = @IsDelivered, [CustomerName] = @CustomerName, [InsurreName] = @InsurreName, [PhoneNumber] = @PhoneNumber, [PoliceAssurance] = @PoliceAssurance, [CompanyName] = @CompanyName, [AssureurID] = @AssureurID, [LieuxdeDepotBorderoID] = @LieuxdeDepotBorderoID, [DeviseID] = @DeviseID, [BranchID] = @BranchID, [OperatorID] = @OperatorID, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [Transport] = @Transport, [PlafondAssurance] = @PlafondAssurance, [NumeroBonPriseEnCharge] = @NumeroBonPriseEnCharge, [VerreAssurance] = @VerreAssurance, [MontureAssurance] = @MontureAssurance, [Plafond] = @Plafond, [TotalMalade] = @TotalMalade, [NumeroFacture] = @NumeroFacture, [BillState] = @BillState, [DatailBill] = @DatailBill, [MntValidate] = @MntValidate, [GestionnaireID] = @GestionnaireID, [ValidateBillDate] = @ValidateBillDate, [BorderoDepotID] = @BorderoDepotID, [DeleteReason] = @DeleteReason, [DeleteBillDate] = @DeleteBillDate, [DeletedByID] = @DeletedByID
                      WHERE ([CustomerOrderID] = @CustomerOrderID)"
            );
            
        }
        
        public override void Down()
        {
            AddColumn("dbo.Prescriptions", "isPrescriptionValidate", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.CustomerOrders", "LieuxdeDepotBorderoID", "dbo.LieuxdeDepotBorderoes");
            DropIndex("dbo.LieuxdeDepotBorderoes", "IX_LieuxdeDepotBordero");
            DropIndex("dbo.CustomerOrders", new[] { "LieuxdeDepotBorderoID" });
            DropColumn("dbo.TillDays", "TillDayCashHand");
            DropColumn("dbo.Prescriptions", "isAuthoriseSale");
            DropColumn("dbo.CustomerOrders", "LieuxdeDepotBorderoID");
            DropColumn("dbo.CustomerOrders", "InsurreName");
            DropColumn("dbo.AuthoriseSales", "IsDilatation");
            DropTable("dbo.LieuxdeDepotBorderoes");
            //throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
