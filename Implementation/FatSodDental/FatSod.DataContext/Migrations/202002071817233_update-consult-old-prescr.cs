namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateconsultoldprescr : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConsultOldPrescrs", "OldPlaintePatient", c => c.String());
            AddColumn("dbo.ConsultOldPrescrs", "OldAcuiteVisuelLID", c => c.Int());
            AddColumn("dbo.ConsultOldPrescrs", "OldAcuiteVisuelPID", c => c.Int());
            CreateIndex("dbo.ConsultOldPrescrs", "OldAcuiteVisuelLID");
            CreateIndex("dbo.ConsultOldPrescrs", "OldAcuiteVisuelPID");
            AddForeignKey("dbo.ConsultOldPrescrs", "OldAcuiteVisuelLID", "dbo.AcuiteVisuelLs", "AcuiteVisuelLID");
            AddForeignKey("dbo.ConsultOldPrescrs", "OldAcuiteVisuelPID", "dbo.AcuiteVisuelPs", "AcuiteVisuelPID");
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
                        AcuiteVisuelPID = p.Int(),
                        OldAcuiteVisuelLID = p.Int(),
                        OldAcuiteVisuelPID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[ConsultOldPrescrs]([ConsultationID], [LAxis], [LAddition], [LIndex], [LCylValue], [LSphValue], [RAxis], [RAddition], [RIndex], [RCylValue], [RSphValue], [CategoryID], [DateDernierConsultation], [IsDilatation], [IsCollyre], [NomCollyre], [PlaintePatient], [OldPlaintePatient], [ConsultByID], [DateConsultOldPres], [HeureConsOldPres], [CustomerNumber], [AcuiteVisuelLID], [AcuiteVisuelPID], [OldAcuiteVisuelLID], [OldAcuiteVisuelPID])
                      VALUES (@ConsultationID, @LAxis, @LAddition, @LIndex, @LCylValue, @LSphValue, @RAxis, @RAddition, @RIndex, @RCylValue, @RSphValue, @CategoryID, @DateDernierConsultation, @IsDilatation, @IsCollyre, @NomCollyre, @PlaintePatient, @OldPlaintePatient, @ConsultByID, @DateConsultOldPres, @HeureConsOldPres, @CustomerNumber, @AcuiteVisuelLID, @AcuiteVisuelPID, @OldAcuiteVisuelLID, @OldAcuiteVisuelPID)
                      
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
                        AcuiteVisuelPID = p.Int(),
                        OldAcuiteVisuelLID = p.Int(),
                        OldAcuiteVisuelPID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[ConsultOldPrescrs]
                      SET [ConsultationID] = @ConsultationID, [LAxis] = @LAxis, [LAddition] = @LAddition, [LIndex] = @LIndex, [LCylValue] = @LCylValue, [LSphValue] = @LSphValue, [RAxis] = @RAxis, [RAddition] = @RAddition, [RIndex] = @RIndex, [RCylValue] = @RCylValue, [RSphValue] = @RSphValue, [CategoryID] = @CategoryID, [DateDernierConsultation] = @DateDernierConsultation, [IsDilatation] = @IsDilatation, [IsCollyre] = @IsCollyre, [NomCollyre] = @NomCollyre, [PlaintePatient] = @PlaintePatient, [OldPlaintePatient] = @OldPlaintePatient, [ConsultByID] = @ConsultByID, [DateConsultOldPres] = @DateConsultOldPres, [HeureConsOldPres] = @HeureConsOldPres, [CustomerNumber] = @CustomerNumber, [AcuiteVisuelLID] = @AcuiteVisuelLID, [AcuiteVisuelPID] = @AcuiteVisuelPID, [OldAcuiteVisuelLID] = @OldAcuiteVisuelLID, [OldAcuiteVisuelPID] = @OldAcuiteVisuelPID
                      WHERE ([ConsultOldPrescrID] = @ConsultOldPrescrID)"
            );
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ConsultOldPrescrs", "OldAcuiteVisuelPID", "dbo.AcuiteVisuelPs");
            DropForeignKey("dbo.ConsultOldPrescrs", "OldAcuiteVisuelLID", "dbo.AcuiteVisuelLs");
            DropIndex("dbo.ConsultOldPrescrs", new[] { "OldAcuiteVisuelPID" });
            DropIndex("dbo.ConsultOldPrescrs", new[] { "OldAcuiteVisuelLID" });
            DropColumn("dbo.ConsultOldPrescrs", "OldAcuiteVisuelPID");
            DropColumn("dbo.ConsultOldPrescrs", "OldAcuiteVisuelLID");
            DropColumn("dbo.ConsultOldPrescrs", "OldPlaintePatient");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
