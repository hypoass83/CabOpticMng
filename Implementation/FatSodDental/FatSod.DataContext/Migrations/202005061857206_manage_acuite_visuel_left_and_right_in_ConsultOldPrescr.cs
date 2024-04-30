namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class manage_acuite_visuel_left_and_right_in_ConsultOldPrescr : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConsultOldPrescrs", "RAcuiteVisuelLID", c => c.Int());
            AddColumn("dbo.ConsultOldPrescrs", "LAcuiteVisuelLID", c => c.Int());
            AddColumn("dbo.ConsultOldPrescrs", "RAcuiteVisuelPID", c => c.Int());
            AddColumn("dbo.ConsultOldPrescrs", "LAcuiteVisuelPID", c => c.Int());
            AddColumn("dbo.ConsultOldPrescrs", "OldRAcuiteVisuelLID", c => c.Int());
            AddColumn("dbo.ConsultOldPrescrs", "OldLAcuiteVisuelLID", c => c.Int());
            AddColumn("dbo.ConsultOldPrescrs", "OldRAcuiteVisuelPID", c => c.Int());
            AddColumn("dbo.ConsultOldPrescrs", "OldLAcuiteVisuelPID", c => c.Int());
            CreateIndex("dbo.ConsultOldPrescrs", "RAcuiteVisuelLID");
            CreateIndex("dbo.ConsultOldPrescrs", "LAcuiteVisuelLID");
            CreateIndex("dbo.ConsultOldPrescrs", "RAcuiteVisuelPID");
            CreateIndex("dbo.ConsultOldPrescrs", "LAcuiteVisuelPID");
            CreateIndex("dbo.ConsultOldPrescrs", "OldRAcuiteVisuelLID");
            CreateIndex("dbo.ConsultOldPrescrs", "OldLAcuiteVisuelLID");
            CreateIndex("dbo.ConsultOldPrescrs", "OldRAcuiteVisuelPID");
            CreateIndex("dbo.ConsultOldPrescrs", "OldLAcuiteVisuelPID");
            AddForeignKey("dbo.ConsultOldPrescrs", "LAcuiteVisuelLID", "dbo.AcuiteVisuelLs", "AcuiteVisuelLID");
            AddForeignKey("dbo.ConsultOldPrescrs", "LAcuiteVisuelPID", "dbo.AcuiteVisuelPs", "AcuiteVisuelPID");
            AddForeignKey("dbo.ConsultOldPrescrs", "OldLAcuiteVisuelLID", "dbo.AcuiteVisuelLs", "AcuiteVisuelLID");
            AddForeignKey("dbo.ConsultOldPrescrs", "OldLAcuiteVisuelPID", "dbo.AcuiteVisuelPs", "AcuiteVisuelPID");
            AddForeignKey("dbo.ConsultOldPrescrs", "OldRAcuiteVisuelLID", "dbo.AcuiteVisuelLs", "AcuiteVisuelLID");
            AddForeignKey("dbo.ConsultOldPrescrs", "OldRAcuiteVisuelPID", "dbo.AcuiteVisuelPs", "AcuiteVisuelPID");
            AddForeignKey("dbo.ConsultOldPrescrs", "RAcuiteVisuelLID", "dbo.AcuiteVisuelLs", "AcuiteVisuelLID");
            AddForeignKey("dbo.ConsultOldPrescrs", "RAcuiteVisuelPID", "dbo.AcuiteVisuelPs", "AcuiteVisuelPID");
            AlterStoredProcedure(
                "dbo.ConsultOldPrescr_Insert",
                p => new
                    {
                        ConsultationID = p.Int(),
                        LAxis = p.String(),
                        LAddition = p.String(),
                        LIndex = p.String(),
                        LCylValue = p.String(),
                        LSphValue = p.String(),
                        RAxis = p.String(),
                        RAddition = p.String(),
                        RIndex = p.String(),
                        RCylValue = p.String(),
                        RSphValue = p.String(),
                        CategoryID = p.Int(),
                        DateDernierConsultation = p.DateTime(),
                        IsDilatation = p.Boolean(),
                        IsCollyre = p.Boolean(),
                        NomCollyre = p.String(),
                        PlaintePatient = p.String(),
                        OldPlaintePatient = p.String(),
                        ConsultByID = p.Int(),
                        DateConsultOldPres = p.DateTime(),
                        HeureConsOldPres = p.String(),
                        CustomerNumber = p.Int(),
                        AcuiteVisuelLID = p.Int(),
                        RAcuiteVisuelLID = p.Int(),
                        LAcuiteVisuelLID = p.Int(),
                        AcuiteVisuelPID = p.Int(),
                        RAcuiteVisuelPID = p.Int(),
                        LAcuiteVisuelPID = p.Int(),
                        OldAcuiteVisuelLID = p.Int(),
                        OldRAcuiteVisuelLID = p.Int(),
                        OldLAcuiteVisuelLID = p.Int(),
                        OldAcuiteVisuelPID = p.Int(),
                        OldRAcuiteVisuelPID = p.Int(),
                        OldLAcuiteVisuelPID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[ConsultOldPrescrs]([ConsultationID], [LAxis], [LAddition], [LIndex], [LCylValue], [LSphValue], [RAxis], [RAddition], [RIndex], [RCylValue], [RSphValue], [CategoryID], [DateDernierConsultation], [IsDilatation], [IsCollyre], [NomCollyre], [PlaintePatient], [OldPlaintePatient], [ConsultByID], [DateConsultOldPres], [HeureConsOldPres], [CustomerNumber], [AcuiteVisuelLID], [RAcuiteVisuelLID], [LAcuiteVisuelLID], [AcuiteVisuelPID], [RAcuiteVisuelPID], [LAcuiteVisuelPID], [OldAcuiteVisuelLID], [OldRAcuiteVisuelLID], [OldLAcuiteVisuelLID], [OldAcuiteVisuelPID], [OldRAcuiteVisuelPID], [OldLAcuiteVisuelPID])
                      VALUES (@ConsultationID, @LAxis, @LAddition, @LIndex, @LCylValue, @LSphValue, @RAxis, @RAddition, @RIndex, @RCylValue, @RSphValue, @CategoryID, @DateDernierConsultation, @IsDilatation, @IsCollyre, @NomCollyre, @PlaintePatient, @OldPlaintePatient, @ConsultByID, @DateConsultOldPres, @HeureConsOldPres, @CustomerNumber, @AcuiteVisuelLID, @RAcuiteVisuelLID, @LAcuiteVisuelLID, @AcuiteVisuelPID, @RAcuiteVisuelPID, @LAcuiteVisuelPID, @OldAcuiteVisuelLID, @OldRAcuiteVisuelLID, @OldLAcuiteVisuelLID, @OldAcuiteVisuelPID, @OldRAcuiteVisuelPID, @OldLAcuiteVisuelPID)
                      
                      DECLARE @ConsultOldPrescrID int
                      SELECT @ConsultOldPrescrID = [ConsultOldPrescrID]
                      FROM [dbo].[ConsultOldPrescrs]
                      WHERE @@ROWCOUNT > 0 AND [ConsultOldPrescrID] = scope_identity()
                      
                      SELECT t0.[ConsultOldPrescrID]
                      FROM [dbo].[ConsultOldPrescrs] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[ConsultOldPrescrID] = @ConsultOldPrescrID"
            );
            
            AlterStoredProcedure(
                "dbo.ConsultOldPrescr_Update",
                p => new
                    {
                        ConsultOldPrescrID = p.Int(),
                        ConsultationID = p.Int(),
                        LAxis = p.String(),
                        LAddition = p.String(),
                        LIndex = p.String(),
                        LCylValue = p.String(),
                        LSphValue = p.String(),
                        RAxis = p.String(),
                        RAddition = p.String(),
                        RIndex = p.String(),
                        RCylValue = p.String(),
                        RSphValue = p.String(),
                        CategoryID = p.Int(),
                        DateDernierConsultation = p.DateTime(),
                        IsDilatation = p.Boolean(),
                        IsCollyre = p.Boolean(),
                        NomCollyre = p.String(),
                        PlaintePatient = p.String(),
                        OldPlaintePatient = p.String(),
                        ConsultByID = p.Int(),
                        DateConsultOldPres = p.DateTime(),
                        HeureConsOldPres = p.String(),
                        CustomerNumber = p.Int(),
                        AcuiteVisuelLID = p.Int(),
                        RAcuiteVisuelLID = p.Int(),
                        LAcuiteVisuelLID = p.Int(),
                        AcuiteVisuelPID = p.Int(),
                        RAcuiteVisuelPID = p.Int(),
                        LAcuiteVisuelPID = p.Int(),
                        OldAcuiteVisuelLID = p.Int(),
                        OldRAcuiteVisuelLID = p.Int(),
                        OldLAcuiteVisuelLID = p.Int(),
                        OldAcuiteVisuelPID = p.Int(),
                        OldRAcuiteVisuelPID = p.Int(),
                        OldLAcuiteVisuelPID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[ConsultOldPrescrs]
                      SET [ConsultationID] = @ConsultationID, [LAxis] = @LAxis, [LAddition] = @LAddition, [LIndex] = @LIndex, [LCylValue] = @LCylValue, [LSphValue] = @LSphValue, [RAxis] = @RAxis, [RAddition] = @RAddition, [RIndex] = @RIndex, [RCylValue] = @RCylValue, [RSphValue] = @RSphValue, [CategoryID] = @CategoryID, [DateDernierConsultation] = @DateDernierConsultation, [IsDilatation] = @IsDilatation, [IsCollyre] = @IsCollyre, [NomCollyre] = @NomCollyre, [PlaintePatient] = @PlaintePatient, [OldPlaintePatient] = @OldPlaintePatient, [ConsultByID] = @ConsultByID, [DateConsultOldPres] = @DateConsultOldPres, [HeureConsOldPres] = @HeureConsOldPres, [CustomerNumber] = @CustomerNumber, [AcuiteVisuelLID] = @AcuiteVisuelLID, [RAcuiteVisuelLID] = @RAcuiteVisuelLID, [LAcuiteVisuelLID] = @LAcuiteVisuelLID, [AcuiteVisuelPID] = @AcuiteVisuelPID, [RAcuiteVisuelPID] = @RAcuiteVisuelPID, [LAcuiteVisuelPID] = @LAcuiteVisuelPID, [OldAcuiteVisuelLID] = @OldAcuiteVisuelLID, [OldRAcuiteVisuelLID] = @OldRAcuiteVisuelLID, [OldLAcuiteVisuelLID] = @OldLAcuiteVisuelLID, [OldAcuiteVisuelPID] = @OldAcuiteVisuelPID, [OldRAcuiteVisuelPID] = @OldRAcuiteVisuelPID, [OldLAcuiteVisuelPID] = @OldLAcuiteVisuelPID
                      WHERE ([ConsultOldPrescrID] = @ConsultOldPrescrID)"
            );
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ConsultOldPrescrs", "RAcuiteVisuelPID", "dbo.AcuiteVisuelPs");
            DropForeignKey("dbo.ConsultOldPrescrs", "RAcuiteVisuelLID", "dbo.AcuiteVisuelLs");
            DropForeignKey("dbo.ConsultOldPrescrs", "OldRAcuiteVisuelPID", "dbo.AcuiteVisuelPs");
            DropForeignKey("dbo.ConsultOldPrescrs", "OldRAcuiteVisuelLID", "dbo.AcuiteVisuelLs");
            DropForeignKey("dbo.ConsultOldPrescrs", "OldLAcuiteVisuelPID", "dbo.AcuiteVisuelPs");
            DropForeignKey("dbo.ConsultOldPrescrs", "OldLAcuiteVisuelLID", "dbo.AcuiteVisuelLs");
            DropForeignKey("dbo.ConsultOldPrescrs", "LAcuiteVisuelPID", "dbo.AcuiteVisuelPs");
            DropForeignKey("dbo.ConsultOldPrescrs", "LAcuiteVisuelLID", "dbo.AcuiteVisuelLs");
            DropIndex("dbo.ConsultOldPrescrs", new[] { "OldLAcuiteVisuelPID" });
            DropIndex("dbo.ConsultOldPrescrs", new[] { "OldRAcuiteVisuelPID" });
            DropIndex("dbo.ConsultOldPrescrs", new[] { "OldLAcuiteVisuelLID" });
            DropIndex("dbo.ConsultOldPrescrs", new[] { "OldRAcuiteVisuelLID" });
            DropIndex("dbo.ConsultOldPrescrs", new[] { "LAcuiteVisuelPID" });
            DropIndex("dbo.ConsultOldPrescrs", new[] { "RAcuiteVisuelPID" });
            DropIndex("dbo.ConsultOldPrescrs", new[] { "LAcuiteVisuelLID" });
            DropIndex("dbo.ConsultOldPrescrs", new[] { "RAcuiteVisuelLID" });
            DropColumn("dbo.ConsultOldPrescrs", "OldLAcuiteVisuelPID");
            DropColumn("dbo.ConsultOldPrescrs", "OldRAcuiteVisuelPID");
            DropColumn("dbo.ConsultOldPrescrs", "OldLAcuiteVisuelLID");
            DropColumn("dbo.ConsultOldPrescrs", "OldRAcuiteVisuelLID");
            DropColumn("dbo.ConsultOldPrescrs", "LAcuiteVisuelPID");
            DropColumn("dbo.ConsultOldPrescrs", "RAcuiteVisuelPID");
            DropColumn("dbo.ConsultOldPrescrs", "LAcuiteVisuelLID");
            DropColumn("dbo.ConsultOldPrescrs", "RAcuiteVisuelLID");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
