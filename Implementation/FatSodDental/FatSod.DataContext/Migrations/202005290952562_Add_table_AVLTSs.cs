namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_table_AVLTSs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AVLTS",
                c => new
                    {
                        AVLTSID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.AVLTSID);
            
            AddColumn("dbo.ConsultOldPrescrs", "RAVLTSID", c => c.Int());
            AddColumn("dbo.ConsultOldPrescrs", "LAVLTSID", c => c.Int());
            AddColumn("dbo.ConsultOldPrescrs", "OldRAVLTSID", c => c.Int());
            AddColumn("dbo.ConsultOldPrescrs", "OldLAVLTSID", c => c.Int());
            CreateIndex("dbo.ConsultOldPrescrs", "RAVLTSID");
            CreateIndex("dbo.ConsultOldPrescrs", "LAVLTSID");
            CreateIndex("dbo.ConsultOldPrescrs", "OldRAVLTSID");
            CreateIndex("dbo.ConsultOldPrescrs", "OldLAVLTSID");
            AddForeignKey("dbo.ConsultOldPrescrs", "LAVLTSID", "dbo.AVLTS", "AVLTSID");
            AddForeignKey("dbo.ConsultOldPrescrs", "OldLAVLTSID", "dbo.AVLTS", "AVLTSID");
            AddForeignKey("dbo.ConsultOldPrescrs", "OldRAVLTSID", "dbo.AVLTS", "AVLTSID");
            AddForeignKey("dbo.ConsultOldPrescrs", "RAVLTSID", "dbo.AVLTS", "AVLTSID");
            CreateStoredProcedure(
                "dbo.AVLTS_Insert",
                p => new
                    {
                        Name = p.String(),
                    },
                body:
                    @"INSERT [dbo].[AVLTS]([Name])
                      VALUES (@Name)
                      
                      DECLARE @AVLTSID int
                      SELECT @AVLTSID = [AVLTSID]
                      FROM [dbo].[AVLTS]
                      WHERE @@ROWCOUNT > 0 AND [AVLTSID] = scope_identity()
                      
                      SELECT t0.[AVLTSID]
                      FROM [dbo].[AVLTS] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[AVLTSID] = @AVLTSID"
            );
            
            CreateStoredProcedure(
                "dbo.AVLTS_Update",
                p => new
                    {
                        AVLTSID = p.Int(),
                        Name = p.String(),
                    },
                body:
                    @"UPDATE [dbo].[AVLTS]
                      SET [Name] = @Name
                      WHERE ([AVLTSID] = @AVLTSID)"
            );
            
            CreateStoredProcedure(
                "dbo.AVLTS_Delete",
                p => new
                    {
                        AVLTSID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[AVLTS]
                      WHERE ([AVLTSID] = @AVLTSID)"
            );
            
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
                        OldRAVLTSID = p.Int(),
                        OldLAVLTSID = p.Int(),
                        OldAcuiteVisuelPID = p.Int(),
                        OldRAcuiteVisuelPID = p.Int(),
                        OldLAcuiteVisuelPID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[ConsultOldPrescrs]([ConsultationID], [LAxis], [LAddition], [LIndex], [LCylValue], [LSphValue], [RAxis], [RAddition], [RIndex], [RCylValue], [RSphValue], [CategoryID], [DateDernierConsultation], [IsDilatation], [IsCollyre], [NomCollyre], [PlaintePatient], [OldPlaintePatient], [ConsultByID], [DateConsultOldPres], [HeureConsOldPres], [CustomerNumber], [AcuiteVisuelLID], [RAcuiteVisuelLID], [LAcuiteVisuelLID], [RAVLTSID], [LAVLTSID], [AcuiteVisuelPID], [RAcuiteVisuelPID], [LAcuiteVisuelPID], [OldAcuiteVisuelLID], [OldRAcuiteVisuelLID], [OldLAcuiteVisuelLID], [OldRAVLTSID], [OldLAVLTSID], [OldAcuiteVisuelPID], [OldRAcuiteVisuelPID], [OldLAcuiteVisuelPID])
                      VALUES (@ConsultationID, @LAxis, @LAddition, @LIndex, @LCylValue, @LSphValue, @RAxis, @RAddition, @RIndex, @RCylValue, @RSphValue, @CategoryID, @DateDernierConsultation, @IsDilatation, @IsCollyre, @NomCollyre, @PlaintePatient, @OldPlaintePatient, @ConsultByID, @DateConsultOldPres, @HeureConsOldPres, @CustomerNumber, @AcuiteVisuelLID, @RAcuiteVisuelLID, @LAcuiteVisuelLID, @RAVLTSID, @LAVLTSID, @AcuiteVisuelPID, @RAcuiteVisuelPID, @LAcuiteVisuelPID, @OldAcuiteVisuelLID, @OldRAcuiteVisuelLID, @OldLAcuiteVisuelLID, @OldRAVLTSID, @OldLAVLTSID, @OldAcuiteVisuelPID, @OldRAcuiteVisuelPID, @OldLAcuiteVisuelPID)
                      
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
                        OldRAVLTSID = p.Int(),
                        OldLAVLTSID = p.Int(),
                        OldAcuiteVisuelPID = p.Int(),
                        OldRAcuiteVisuelPID = p.Int(),
                        OldLAcuiteVisuelPID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[ConsultOldPrescrs]
                      SET [ConsultationID] = @ConsultationID, [LAxis] = @LAxis, [LAddition] = @LAddition, [LIndex] = @LIndex, [LCylValue] = @LCylValue, [LSphValue] = @LSphValue, [RAxis] = @RAxis, [RAddition] = @RAddition, [RIndex] = @RIndex, [RCylValue] = @RCylValue, [RSphValue] = @RSphValue, [CategoryID] = @CategoryID, [DateDernierConsultation] = @DateDernierConsultation, [IsDilatation] = @IsDilatation, [IsCollyre] = @IsCollyre, [NomCollyre] = @NomCollyre, [PlaintePatient] = @PlaintePatient, [OldPlaintePatient] = @OldPlaintePatient, [ConsultByID] = @ConsultByID, [DateConsultOldPres] = @DateConsultOldPres, [HeureConsOldPres] = @HeureConsOldPres, [CustomerNumber] = @CustomerNumber, [AcuiteVisuelLID] = @AcuiteVisuelLID, [RAcuiteVisuelLID] = @RAcuiteVisuelLID, [LAcuiteVisuelLID] = @LAcuiteVisuelLID, [RAVLTSID] = @RAVLTSID, [LAVLTSID] = @LAVLTSID, [AcuiteVisuelPID] = @AcuiteVisuelPID, [RAcuiteVisuelPID] = @RAcuiteVisuelPID, [LAcuiteVisuelPID] = @LAcuiteVisuelPID, [OldAcuiteVisuelLID] = @OldAcuiteVisuelLID, [OldRAcuiteVisuelLID] = @OldRAcuiteVisuelLID, [OldLAcuiteVisuelLID] = @OldLAcuiteVisuelLID, [OldRAVLTSID] = @OldRAVLTSID, [OldLAVLTSID] = @OldLAVLTSID, [OldAcuiteVisuelPID] = @OldAcuiteVisuelPID, [OldRAcuiteVisuelPID] = @OldRAcuiteVisuelPID, [OldLAcuiteVisuelPID] = @OldLAcuiteVisuelPID
                      WHERE ([ConsultOldPrescrID] = @ConsultOldPrescrID)"
            );

            //initialise AVL And AVP Table with Data
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('1/10')");
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('2/10')");
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('3/10')");
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('4/10')");
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('5/10')");
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('6/10')");
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('7/10')");
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('8/10')");
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('9/10')");
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('10/10')");
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('11/10')");
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('12/10')");
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('13/10')");
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('14/10')");
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('15/10')");
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('VBLM')");
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('CLD à 1m')");
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('CLD à 2m')");
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('PL+')");
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('PL-')");
            Sql(@"INSERT INTO dbo.AVLTS(Name) VALUES ('TS')");
        }
        
        public override void Down()
        {
            DropStoredProcedure("dbo.AVLTS_Delete");
            DropStoredProcedure("dbo.AVLTS_Update");
            DropStoredProcedure("dbo.AVLTS_Insert");
            DropForeignKey("dbo.ConsultOldPrescrs", "RAVLTSID", "dbo.AVLTS");
            DropForeignKey("dbo.ConsultOldPrescrs", "OldRAVLTSID", "dbo.AVLTS");
            DropForeignKey("dbo.ConsultOldPrescrs", "OldLAVLTSID", "dbo.AVLTS");
            DropForeignKey("dbo.ConsultOldPrescrs", "LAVLTSID", "dbo.AVLTS");
            DropIndex("dbo.ConsultOldPrescrs", new[] { "OldLAVLTSID" });
            DropIndex("dbo.ConsultOldPrescrs", new[] { "OldRAVLTSID" });
            DropIndex("dbo.ConsultOldPrescrs", new[] { "LAVLTSID" });
            DropIndex("dbo.ConsultOldPrescrs", new[] { "RAVLTSID" });
            DropColumn("dbo.ConsultOldPrescrs", "OldLAVLTSID");
            DropColumn("dbo.ConsultOldPrescrs", "OldRAVLTSID");
            DropColumn("dbo.ConsultOldPrescrs", "LAVLTSID");
            DropColumn("dbo.ConsultOldPrescrs", "RAVLTSID");
            DropTable("dbo.AVLTS");
            //throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
