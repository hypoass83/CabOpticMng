namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class oldtsavl_remove : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ConsultOldPrescrs", "OldLAVLTSID", "dbo.AVLTS");
            DropForeignKey("dbo.ConsultOldPrescrs", "OldRAVLTSID", "dbo.AVLTS");
            DropIndex("dbo.ConsultOldPrescrs", new[] { "OldRAVLTSID" });
            DropIndex("dbo.ConsultOldPrescrs", new[] { "OldLAVLTSID" });
            DropColumn("dbo.ConsultOldPrescrs", "OldRAVLTSID");
            DropColumn("dbo.ConsultOldPrescrs", "OldLAVLTSID");
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
                        RAVLTSID = p.Int(),
                        LAVLTSID = p.Int(),
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
                    @"INSERT [dbo].[ConsultOldPrescrs]([ConsultationID], [LAxis], [LAddition], [LIndex], [LCylValue], [LSphValue], [RAxis], [RAddition], [RIndex], [RCylValue], [RSphValue], [CategoryID], [DateDernierConsultation], [IsDilatation], [IsCollyre], [NomCollyre], [PlaintePatient], [OldPlaintePatient], [ConsultByID], [DateConsultOldPres], [HeureConsOldPres], [CustomerNumber], [AcuiteVisuelLID], [RAcuiteVisuelLID], [LAcuiteVisuelLID], [RAVLTSID], [LAVLTSID], [AcuiteVisuelPID], [RAcuiteVisuelPID], [LAcuiteVisuelPID], [OldAcuiteVisuelLID], [OldRAcuiteVisuelLID], [OldLAcuiteVisuelLID], [OldAcuiteVisuelPID], [OldRAcuiteVisuelPID], [OldLAcuiteVisuelPID])
                      VALUES (@ConsultationID, @LAxis, @LAddition, @LIndex, @LCylValue, @LSphValue, @RAxis, @RAddition, @RIndex, @RCylValue, @RSphValue, @CategoryID, @DateDernierConsultation, @IsDilatation, @IsCollyre, @NomCollyre, @PlaintePatient, @OldPlaintePatient, @ConsultByID, @DateConsultOldPres, @HeureConsOldPres, @CustomerNumber, @AcuiteVisuelLID, @RAcuiteVisuelLID, @LAcuiteVisuelLID, @RAVLTSID, @LAVLTSID, @AcuiteVisuelPID, @RAcuiteVisuelPID, @LAcuiteVisuelPID, @OldAcuiteVisuelLID, @OldRAcuiteVisuelLID, @OldLAcuiteVisuelLID, @OldAcuiteVisuelPID, @OldRAcuiteVisuelPID, @OldLAcuiteVisuelPID)
                      
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
                        RAVLTSID = p.Int(),
                        LAVLTSID = p.Int(),
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
                      SET [ConsultationID] = @ConsultationID, [LAxis] = @LAxis, [LAddition] = @LAddition, [LIndex] = @LIndex, [LCylValue] = @LCylValue, [LSphValue] = @LSphValue, [RAxis] = @RAxis, [RAddition] = @RAddition, [RIndex] = @RIndex, [RCylValue] = @RCylValue, [RSphValue] = @RSphValue, [CategoryID] = @CategoryID, [DateDernierConsultation] = @DateDernierConsultation, [IsDilatation] = @IsDilatation, [IsCollyre] = @IsCollyre, [NomCollyre] = @NomCollyre, [PlaintePatient] = @PlaintePatient, [OldPlaintePatient] = @OldPlaintePatient, [ConsultByID] = @ConsultByID, [DateConsultOldPres] = @DateConsultOldPres, [HeureConsOldPres] = @HeureConsOldPres, [CustomerNumber] = @CustomerNumber, [AcuiteVisuelLID] = @AcuiteVisuelLID, [RAcuiteVisuelLID] = @RAcuiteVisuelLID, [LAcuiteVisuelLID] = @LAcuiteVisuelLID, [RAVLTSID] = @RAVLTSID, [LAVLTSID] = @LAVLTSID, [AcuiteVisuelPID] = @AcuiteVisuelPID, [RAcuiteVisuelPID] = @RAcuiteVisuelPID, [LAcuiteVisuelPID] = @LAcuiteVisuelPID, [OldAcuiteVisuelLID] = @OldAcuiteVisuelLID, [OldRAcuiteVisuelLID] = @OldRAcuiteVisuelLID, [OldLAcuiteVisuelLID] = @OldLAcuiteVisuelLID, [OldAcuiteVisuelPID] = @OldAcuiteVisuelPID, [OldRAcuiteVisuelPID] = @OldRAcuiteVisuelPID, [OldLAcuiteVisuelPID] = @OldLAcuiteVisuelPID
                      WHERE ([ConsultOldPrescrID] = @ConsultOldPrescrID)"
            );
            
        }
        
        public override void Down()
        {
            AddColumn("dbo.ConsultOldPrescrs", "OldLAVLTSID", c => c.Int());
            AddColumn("dbo.ConsultOldPrescrs", "OldRAVLTSID", c => c.Int());
            CreateIndex("dbo.ConsultOldPrescrs", "OldLAVLTSID");
            CreateIndex("dbo.ConsultOldPrescrs", "OldRAVLTSID");
            AddForeignKey("dbo.ConsultOldPrescrs", "OldRAVLTSID", "dbo.AVLTS", "AVLTSID");
            AddForeignKey("dbo.ConsultOldPrescrs", "OldLAVLTSID", "dbo.AVLTS", "AVLTSID");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
