namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_rem_entity_Prescription_to_PrescriptionLStep : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Prescriptions", "ConsultationID", "dbo.Consultations");
            DropForeignKey("dbo.Prescriptions", "OperatorID", "dbo.Users");
            DropForeignKey("dbo.Prescriptions", "PostByID", "dbo.Users");
            DropForeignKey("dbo.PrescriptionLines", "PrescriptionID", "dbo.Prescriptions");
            DropForeignKey("dbo.PrescriptionLines", "ProductID", "dbo.Products");
            DropIndex("dbo.PrescriptionLines", new[] { "PrescriptionID" });
            DropIndex("dbo.PrescriptionLines", new[] { "ProductID" });
            DropIndex("dbo.Prescriptions", new[] { "ConsultationID" });
            DropIndex("dbo.Prescriptions", new[] { "OperatorID" });
            DropIndex("dbo.Prescriptions", new[] { "PostByID" });
            CreateTable(
                "dbo.PrescriptionLSteps",
                c => new
                    {
                        PrescriptionLStepID = c.Int(nullable: false, identity: true),
                        DateHeurePrescriptionLStep = c.DateTime(nullable: false),
                        DatePrescriptionLStep = c.DateTime(nullable: false),
                        ConsultationID = c.Int(nullable: false),
                        ConsultByID = c.Int(),
                        Remarque = c.String(),
                        MedecinTraitant = c.String(),
                        DateRdv = c.DateTime(nullable: false),
                        PrescriptionCollyre = c.Boolean(nullable: false),
                        CollyreName = c.String(),
                    })
                .PrimaryKey(t => t.PrescriptionLStepID)
                .ForeignKey("dbo.Consultations", t => t.ConsultationID)
                .ForeignKey("dbo.Users", t => t.ConsultByID)
                .Index(t => t.ConsultationID)
                .Index(t => t.ConsultByID);
            
            DropTable("dbo.PrescriptionLines");
            DropTable("dbo.Prescriptions");
            CreateStoredProcedure(
                "dbo.PrescriptionLStep_Insert",
                p => new
                    {
                        DateHeurePrescriptionLStep = p.DateTime(),
                        DatePrescriptionLStep = p.DateTime(),
                        ConsultationID = p.Int(),
                        ConsultByID = p.Int(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        DateRdv = p.DateTime(),
                        PrescriptionCollyre = p.Boolean(),
                        CollyreName = p.String(),
                    },
                body:
                    @"INSERT [dbo].[PrescriptionLSteps]([DateHeurePrescriptionLStep], [DatePrescriptionLStep], [ConsultationID], [ConsultByID], [Remarque], [MedecinTraitant], [DateRdv], [PrescriptionCollyre], [CollyreName])
                      VALUES (@DateHeurePrescriptionLStep, @DatePrescriptionLStep, @ConsultationID, @ConsultByID, @Remarque, @MedecinTraitant, @DateRdv, @PrescriptionCollyre, @CollyreName)
                      
                      DECLARE @PrescriptionLStepID int
                      SELECT @PrescriptionLStepID = [PrescriptionLStepID]
                      FROM [dbo].[PrescriptionLSteps]
                      WHERE @@ROWCOUNT > 0 AND [PrescriptionLStepID] = scope_identity()
                      
                      SELECT t0.[PrescriptionLStepID]
                      FROM [dbo].[PrescriptionLSteps] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[PrescriptionLStepID] = @PrescriptionLStepID"
            );
            
            CreateStoredProcedure(
                "dbo.PrescriptionLStep_Update",
                p => new
                    {
                        PrescriptionLStepID = p.Int(),
                        DateHeurePrescriptionLStep = p.DateTime(),
                        DatePrescriptionLStep = p.DateTime(),
                        ConsultationID = p.Int(),
                        ConsultByID = p.Int(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        DateRdv = p.DateTime(),
                        PrescriptionCollyre = p.Boolean(),
                        CollyreName = p.String(),
                    },
                body:
                    @"UPDATE [dbo].[PrescriptionLSteps]
                      SET [DateHeurePrescriptionLStep] = @DateHeurePrescriptionLStep, [DatePrescriptionLStep] = @DatePrescriptionLStep, [ConsultationID] = @ConsultationID, [ConsultByID] = @ConsultByID, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [DateRdv] = @DateRdv, [PrescriptionCollyre] = @PrescriptionCollyre, [CollyreName] = @CollyreName
                      WHERE ([PrescriptionLStepID] = @PrescriptionLStepID)"
            );
            
            CreateStoredProcedure(
                "dbo.PrescriptionLStep_Delete",
                p => new
                    {
                        PrescriptionLStepID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[PrescriptionLSteps]
                      WHERE ([PrescriptionLStepID] = @PrescriptionLStepID)"
            );
            
        }
        
        public override void Down()
        {
            DropStoredProcedure("dbo.PrescriptionLStep_Delete");
            DropStoredProcedure("dbo.PrescriptionLStep_Update");
            DropStoredProcedure("dbo.PrescriptionLStep_Insert");
            CreateTable(
                "dbo.Prescriptions",
                c => new
                    {
                        PrescriptionID = c.Int(nullable: false, identity: true),
                        DateHeurePrescription = c.DateTime(nullable: false),
                        DatePrescription = c.DateTime(nullable: false),
                        isAuthoriseSale = c.Boolean(nullable: false),
                        ConsultationID = c.Int(nullable: false),
                        OperatorID = c.Int(),
                        PostByID = c.Int(),
                        Remarque = c.String(),
                        MedecinTraitant = c.String(),
                        Plainte = c.String(),
                        isDilation = c.Boolean(nullable: false),
                        CodeDilation = c.String(maxLength: 50),
                        DateRdv = c.DateTime(),
                        PrescriptionCollyre = c.Boolean(nullable: false),
                        CollyreName = c.String(),
                    })
                .PrimaryKey(t => t.PrescriptionID);
            
            CreateTable(
                "dbo.PrescriptionLines",
                c => new
                    {
                        PrescriptionLineID = c.Int(nullable: false, identity: true),
                        PrescriptionID = c.Int(nullable: false),
                        SpecialOrderLineCode = c.String(),
                        Axis = c.String(),
                        Addition = c.String(),
                        Index = c.String(),
                        LensNumberCylindricalValue = c.String(),
                        LensNumberSphericalValue = c.String(),
                        ProductID = c.Int(nullable: false),
                        OeilDroiteGauche = c.Int(nullable: false),
                        SupplyingName = c.String(),
                    })
                .PrimaryKey(t => t.PrescriptionLineID);
            
            DropForeignKey("dbo.PrescriptionLSteps", "ConsultByID", "dbo.Users");
            DropForeignKey("dbo.PrescriptionLSteps", "ConsultationID", "dbo.Consultations");
            DropIndex("dbo.PrescriptionLSteps", new[] { "ConsultByID" });
            DropIndex("dbo.PrescriptionLSteps", new[] { "ConsultationID" });
            DropTable("dbo.PrescriptionLSteps");
            CreateIndex("dbo.Prescriptions", "PostByID");
            CreateIndex("dbo.Prescriptions", "OperatorID");
            CreateIndex("dbo.Prescriptions", "ConsultationID");
            CreateIndex("dbo.PrescriptionLines", "ProductID");
            CreateIndex("dbo.PrescriptionLines", "PrescriptionID");
            AddForeignKey("dbo.PrescriptionLines", "ProductID", "dbo.Products", "ProductID");
            AddForeignKey("dbo.PrescriptionLines", "PrescriptionID", "dbo.Prescriptions", "PrescriptionID");
            AddForeignKey("dbo.Prescriptions", "PostByID", "dbo.Users", "GlobalPersonID");
            AddForeignKey("dbo.Prescriptions", "OperatorID", "dbo.Users", "GlobalPersonID");
            AddForeignKey("dbo.Prescriptions", "ConsultationID", "dbo.Consultations", "ConsultationID");
        }
    }
}
