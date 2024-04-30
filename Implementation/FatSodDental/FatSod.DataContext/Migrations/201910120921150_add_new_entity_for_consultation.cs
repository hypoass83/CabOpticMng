namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_new_entity_for_consultation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConsultDilPrescs",
                c => new
                    {
                        ConsultDilPrescID = c.Int(nullable: false, identity: true),
                        ConsultationID = c.Int(nullable: false),
                        CustomerNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ConsultDilPrescID)
                .ForeignKey("dbo.Consultations", t => t.ConsultationID)
                .Index(t => t.ConsultationID);
            
            CreateTable(
                "dbo.AcuiteVisuelLs",
                c => new
                    {
                        AcuiteVisuelLID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.AcuiteVisuelLID);
            
            CreateTable(
                "dbo.AcuiteVisuelPs",
                c => new
                    {
                        AcuiteVisuelPID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.AcuiteVisuelPID);
            
            CreateTable(
                "dbo.ConsultDilatations",
                c => new
                    {
                        ConsultDilatationID = c.Int(nullable: false, identity: true),
                        ConsultDilPrescID = c.Int(nullable: false),
                        CodeDilation = c.String(),
                        ConsultByID = c.Int(nullable: false),
                        HeureConsultDilatation = c.String(),
                        DateDilation = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ConsultDilatationID)
                .ForeignKey("dbo.Users", t => t.ConsultByID)
                .ForeignKey("dbo.ConsultDilPrescs", t => t.ConsultDilPrescID)
                .Index(t => t.ConsultDilPrescID)
                .Index(t => t.ConsultByID);
            
            CreateTable(
                "dbo.ConsultLensPrescriptions",
                c => new
                    {
                        ConsultLensPrescriptionID = c.Int(nullable: false, identity: true),
                        ConsultDilPrescID = c.Int(nullable: false),
                        LAxis = c.String(),
                        LAddition = c.String(),
                        LIndex = c.String(),
                        LCylValue = c.String(),
                        LSphValue = c.String(),
                        RAxis = c.String(),
                        RAddition = c.String(),
                        RIndex = c.String(),
                        RCylValue = c.String(),
                        RSphValue = c.String(),
                        CategoryID = c.Int(nullable: false),
                        SupplyingName = c.String(),
                        DatePrescription = c.DateTime(nullable: false),
                        ConsultByID = c.Int(nullable: false),
                        HeureLensPrescription = c.String(),
                    })
                .PrimaryKey(t => t.ConsultLensPrescriptionID)
                .ForeignKey("dbo.Categories", t => t.CategoryID)
                .ForeignKey("dbo.Users", t => t.ConsultByID)
                .ForeignKey("dbo.ConsultDilPrescs", t => t.ConsultDilPrescID)
                .Index(t => t.ConsultDilPrescID)
                .Index(t => t.CategoryID)
                .Index(t => t.ConsultByID);
            
            CreateTable(
                "dbo.ConsultOldPrescrs",
                c => new
                    {
                        ConsultOldPrescrID = c.Int(nullable: false, identity: true),
                        ConsultationID = c.Int(nullable: false),
                        LAxis = c.String(),
                        LAddition = c.String(),
                        LIndex = c.String(),
                        LCylValue = c.String(),
                        LSphValue = c.String(),
                        RAxis = c.String(),
                        RAddition = c.String(),
                        RIndex = c.String(),
                        RCylValue = c.String(),
                        RSphValue = c.String(),
                        CategoryID = c.Int(),
                        DateDernierConsultation = c.DateTime(nullable: false),
                        IsDilatation = c.Boolean(nullable: false),
                        IsCollyre = c.Boolean(nullable: false),
                        NomCollyre = c.String(),
                        PlaintePatient = c.String(),
                        ConsultByID = c.Int(nullable: false),
                        DateConsultOldPres = c.DateTime(nullable: false),
                        HeureConsOldPres = c.String(),
                        CustomerNumber = c.Int(nullable: false),
                        AcuiteVisuelLID = c.Int(),
                        AcuiteVisuelPID = c.Int(),
                    })
                .PrimaryKey(t => t.ConsultOldPrescrID)
                .ForeignKey("dbo.AcuiteVisuelLs", t => t.AcuiteVisuelLID)
                .ForeignKey("dbo.AcuiteVisuelPs", t => t.AcuiteVisuelPID)
                .ForeignKey("dbo.Categories", t => t.CategoryID)
                .ForeignKey("dbo.Consultations", t => t.ConsultationID)
                .ForeignKey("dbo.Users", t => t.ConsultByID)
                .Index(t => t.ConsultationID)
                .Index(t => t.CategoryID)
                .Index(t => t.ConsultByID)
                .Index(t => t.AcuiteVisuelLID)
                .Index(t => t.AcuiteVisuelPID);
            
            CreateTable(
                "dbo.ConsultPersonalMedHistoes",
                c => new
                    {
                        ConsultPersonalMedHistoID = c.Int(nullable: false, identity: true),
                        ConsultationID = c.Int(nullable: false),
                        ConsultByID = c.Int(nullable: false),
                        DateConsultPersonalMedHisto = c.DateTime(nullable: false),
                        HeureConsultPersonalMedHisto = c.String(),
                        CustomerNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ConsultPersonalMedHistoID)
                .ForeignKey("dbo.Consultations", t => t.ConsultationID)
                .ForeignKey("dbo.Users", t => t.ConsultByID)
                .Index(t => t.ConsultationID)
                .Index(t => t.ConsultByID);
            
            CreateTable(
                "dbo.ConsultPrescrLastSteps",
                c => new
                    {
                        ConsultPrescrLastStepID = c.Int(nullable: false, identity: true),
                        ConsultationID = c.Int(nullable: false),
                        DateNextConsultation = c.DateTime(nullable: false),
                        IsCollyre = c.Boolean(nullable: false),
                        NomCollyre = c.String(),
                        Remark = c.String(),
                        ConsultByID = c.Int(nullable: false),
                        HeureConsLastStep = c.String(),
                        CustomerNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ConsultPrescrLastStepID)
                .ForeignKey("dbo.Consultations", t => t.ConsultationID)
                .ForeignKey("dbo.Users", t => t.ConsultByID)
                .Index(t => t.ConsultationID)
                .Index(t => t.ConsultByID);
            
            AddColumn("dbo.Sales", "ConsultDilPrescID", c => c.Int());
            AddColumn("dbo.AuthoriseSales", "ConsultDilPrescID", c => c.Int());
            CreateIndex("dbo.Sales", "ConsultDilPrescID");
            CreateIndex("dbo.AuthoriseSales", "ConsultDilPrescID");
            AddForeignKey("dbo.AuthoriseSales", "ConsultDilPrescID", "dbo.ConsultDilPrescs", "ConsultDilPrescID");
            AddForeignKey("dbo.Sales", "ConsultDilPrescID", "dbo.ConsultDilPrescs", "ConsultDilPrescID");
            CreateStoredProcedure(
                "dbo.ConsultDilPresc_Insert",
                p => new
                    {
                        ConsultationID = p.Int(),
                        CustomerNumber = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[ConsultDilPrescs]([ConsultationID], [CustomerNumber])
                      VALUES (@ConsultationID, @CustomerNumber)
                      
                      DECLARE @ConsultDilPrescID int
                      SELECT @ConsultDilPrescID = [ConsultDilPrescID]
                      FROM [dbo].[ConsultDilPrescs]
                      WHERE @@ROWCOUNT > 0 AND [ConsultDilPrescID] = scope_identity()
                      
                      SELECT t0.[ConsultDilPrescID]
                      FROM [dbo].[ConsultDilPrescs] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[ConsultDilPrescID] = @ConsultDilPrescID"
            );
            
            CreateStoredProcedure(
                "dbo.ConsultDilPresc_Update",
                p => new
                    {
                        ConsultDilPrescID = p.Int(),
                        ConsultationID = p.Int(),
                        CustomerNumber = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[ConsultDilPrescs]
                      SET [ConsultationID] = @ConsultationID, [CustomerNumber] = @CustomerNumber
                      WHERE ([ConsultDilPrescID] = @ConsultDilPrescID)"
            );
            
            CreateStoredProcedure(
                "dbo.ConsultDilPresc_Delete",
                p => new
                    {
                        ConsultDilPrescID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[ConsultDilPrescs]
                      WHERE ([ConsultDilPrescID] = @ConsultDilPrescID)"
            );
            
            CreateStoredProcedure(
                "dbo.AcuiteVisuelL_Insert",
                p => new
                    {
                        Name = p.String(),
                    },
                body:
                    @"INSERT [dbo].[AcuiteVisuelLs]([Name])
                      VALUES (@Name)
                      
                      DECLARE @AcuiteVisuelLID int
                      SELECT @AcuiteVisuelLID = [AcuiteVisuelLID]
                      FROM [dbo].[AcuiteVisuelLs]
                      WHERE @@ROWCOUNT > 0 AND [AcuiteVisuelLID] = scope_identity()
                      
                      SELECT t0.[AcuiteVisuelLID]
                      FROM [dbo].[AcuiteVisuelLs] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[AcuiteVisuelLID] = @AcuiteVisuelLID"
            );
            
            CreateStoredProcedure(
                "dbo.AcuiteVisuelL_Update",
                p => new
                    {
                        AcuiteVisuelLID = p.Int(),
                        Name = p.String(),
                    },
                body:
                    @"UPDATE [dbo].[AcuiteVisuelLs]
                      SET [Name] = @Name
                      WHERE ([AcuiteVisuelLID] = @AcuiteVisuelLID)"
            );
            
            CreateStoredProcedure(
                "dbo.AcuiteVisuelL_Delete",
                p => new
                    {
                        AcuiteVisuelLID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[AcuiteVisuelLs]
                      WHERE ([AcuiteVisuelLID] = @AcuiteVisuelLID)"
            );
            
            CreateStoredProcedure(
                "dbo.AcuiteVisuelP_Insert",
                p => new
                    {
                        Name = p.String(),
                    },
                body:
                    @"INSERT [dbo].[AcuiteVisuelPs]([Name])
                      VALUES (@Name)
                      
                      DECLARE @AcuiteVisuelPID int
                      SELECT @AcuiteVisuelPID = [AcuiteVisuelPID]
                      FROM [dbo].[AcuiteVisuelPs]
                      WHERE @@ROWCOUNT > 0 AND [AcuiteVisuelPID] = scope_identity()
                      
                      SELECT t0.[AcuiteVisuelPID]
                      FROM [dbo].[AcuiteVisuelPs] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[AcuiteVisuelPID] = @AcuiteVisuelPID"
            );
            
            CreateStoredProcedure(
                "dbo.AcuiteVisuelP_Update",
                p => new
                    {
                        AcuiteVisuelPID = p.Int(),
                        Name = p.String(),
                    },
                body:
                    @"UPDATE [dbo].[AcuiteVisuelPs]
                      SET [Name] = @Name
                      WHERE ([AcuiteVisuelPID] = @AcuiteVisuelPID)"
            );
            
            CreateStoredProcedure(
                "dbo.AcuiteVisuelP_Delete",
                p => new
                    {
                        AcuiteVisuelPID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[AcuiteVisuelPs]
                      WHERE ([AcuiteVisuelPID] = @AcuiteVisuelPID)"
            );
            
            CreateStoredProcedure(
                "dbo.ConsultDilatation_Insert",
                p => new
                    {
                        ConsultDilPrescID = p.Int(),
                        CodeDilation = p.String(),
                        ConsultByID = p.Int(),
                        HeureConsultDilatation = p.String(),
                        DateDilation = p.DateTime(),
                    },
                body:
                    @"INSERT [dbo].[ConsultDilatations]([ConsultDilPrescID], [CodeDilation], [ConsultByID], [HeureConsultDilatation], [DateDilation])
                      VALUES (@ConsultDilPrescID, @CodeDilation, @ConsultByID, @HeureConsultDilatation, @DateDilation)
                      
                      DECLARE @ConsultDilatationID int
                      SELECT @ConsultDilatationID = [ConsultDilatationID]
                      FROM [dbo].[ConsultDilatations]
                      WHERE @@ROWCOUNT > 0 AND [ConsultDilatationID] = scope_identity()
                      
                      SELECT t0.[ConsultDilatationID]
                      FROM [dbo].[ConsultDilatations] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[ConsultDilatationID] = @ConsultDilatationID"
            );
            
            CreateStoredProcedure(
                "dbo.ConsultDilatation_Update",
                p => new
                    {
                        ConsultDilatationID = p.Int(),
                        ConsultDilPrescID = p.Int(),
                        CodeDilation = p.String(),
                        ConsultByID = p.Int(),
                        HeureConsultDilatation = p.String(),
                        DateDilation = p.DateTime(),
                    },
                body:
                    @"UPDATE [dbo].[ConsultDilatations]
                      SET [ConsultDilPrescID] = @ConsultDilPrescID, [CodeDilation] = @CodeDilation, [ConsultByID] = @ConsultByID, [HeureConsultDilatation] = @HeureConsultDilatation, [DateDilation] = @DateDilation
                      WHERE ([ConsultDilatationID] = @ConsultDilatationID)"
            );
            
            CreateStoredProcedure(
                "dbo.ConsultDilatation_Delete",
                p => new
                    {
                        ConsultDilatationID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[ConsultDilatations]
                      WHERE ([ConsultDilatationID] = @ConsultDilatationID)"
            );
            
            CreateStoredProcedure(
                "dbo.ConsultLensPrescription_Insert",
                p => new
                    {
                        ConsultDilPrescID = p.Int(),
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
                        SupplyingName = p.String(),
                        DatePrescription = p.DateTime(),
                        ConsultByID = p.Int(),
                        HeureLensPrescription = p.String(),
                    },
                body:
                    @"INSERT [dbo].[ConsultLensPrescriptions]([ConsultDilPrescID], [LAxis], [LAddition], [LIndex], [LCylValue], [LSphValue], [RAxis], [RAddition], [RIndex], [RCylValue], [RSphValue], [CategoryID], [SupplyingName], [DatePrescription], [ConsultByID], [HeureLensPrescription])
                      VALUES (@ConsultDilPrescID, @LAxis, @LAddition, @LIndex, @LCylValue, @LSphValue, @RAxis, @RAddition, @RIndex, @RCylValue, @RSphValue, @CategoryID, @SupplyingName, @DatePrescription, @ConsultByID, @HeureLensPrescription)
                      
                      DECLARE @ConsultLensPrescriptionID int
                      SELECT @ConsultLensPrescriptionID = [ConsultLensPrescriptionID]
                      FROM [dbo].[ConsultLensPrescriptions]
                      WHERE @@ROWCOUNT > 0 AND [ConsultLensPrescriptionID] = scope_identity()
                      
                      SELECT t0.[ConsultLensPrescriptionID]
                      FROM [dbo].[ConsultLensPrescriptions] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[ConsultLensPrescriptionID] = @ConsultLensPrescriptionID"
            );
            
            CreateStoredProcedure(
                "dbo.ConsultLensPrescription_Update",
                p => new
                    {
                        ConsultLensPrescriptionID = p.Int(),
                        ConsultDilPrescID = p.Int(),
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
                        SupplyingName = p.String(),
                        DatePrescription = p.DateTime(),
                        ConsultByID = p.Int(),
                        HeureLensPrescription = p.String(),
                    },
                body:
                    @"UPDATE [dbo].[ConsultLensPrescriptions]
                      SET [ConsultDilPrescID] = @ConsultDilPrescID, [LAxis] = @LAxis, [LAddition] = @LAddition, [LIndex] = @LIndex, [LCylValue] = @LCylValue, [LSphValue] = @LSphValue, [RAxis] = @RAxis, [RAddition] = @RAddition, [RIndex] = @RIndex, [RCylValue] = @RCylValue, [RSphValue] = @RSphValue, [CategoryID] = @CategoryID, [SupplyingName] = @SupplyingName, [DatePrescription] = @DatePrescription, [ConsultByID] = @ConsultByID, [HeureLensPrescription] = @HeureLensPrescription
                      WHERE ([ConsultLensPrescriptionID] = @ConsultLensPrescriptionID)"
            );
            
            CreateStoredProcedure(
                "dbo.ConsultLensPrescription_Delete",
                p => new
                    {
                        ConsultLensPrescriptionID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[ConsultLensPrescriptions]
                      WHERE ([ConsultLensPrescriptionID] = @ConsultLensPrescriptionID)"
            );
            
            CreateStoredProcedure(
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
                        ConsultByID = p.Int(),
                        DateConsultOldPres = p.DateTime(),
                        HeureConsOldPres = p.String(),
                        CustomerNumber = p.Int(),
                        AcuiteVisuelLID = p.Int(),
                        AcuiteVisuelPID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[ConsultOldPrescrs]([ConsultationID], [LAxis], [LAddition], [LIndex], [LCylValue], [LSphValue], [RAxis], [RAddition], [RIndex], [RCylValue], [RSphValue], [CategoryID], [DateDernierConsultation], [IsDilatation], [IsCollyre], [NomCollyre], [PlaintePatient], [ConsultByID], [DateConsultOldPres], [HeureConsOldPres], [CustomerNumber], [AcuiteVisuelLID], [AcuiteVisuelPID])
                      VALUES (@ConsultationID, @LAxis, @LAddition, @LIndex, @LCylValue, @LSphValue, @RAxis, @RAddition, @RIndex, @RCylValue, @RSphValue, @CategoryID, @DateDernierConsultation, @IsDilatation, @IsCollyre, @NomCollyre, @PlaintePatient, @ConsultByID, @DateConsultOldPres, @HeureConsOldPres, @CustomerNumber, @AcuiteVisuelLID, @AcuiteVisuelPID)
                      
                      DECLARE @ConsultOldPrescrID int
                      SELECT @ConsultOldPrescrID = [ConsultOldPrescrID]
                      FROM [dbo].[ConsultOldPrescrs]
                      WHERE @@ROWCOUNT > 0 AND [ConsultOldPrescrID] = scope_identity()
                      
                      SELECT t0.[ConsultOldPrescrID]
                      FROM [dbo].[ConsultOldPrescrs] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[ConsultOldPrescrID] = @ConsultOldPrescrID"
            );
            
            CreateStoredProcedure(
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
                        ConsultByID = p.Int(),
                        DateConsultOldPres = p.DateTime(),
                        HeureConsOldPres = p.String(),
                        CustomerNumber = p.Int(),
                        AcuiteVisuelLID = p.Int(),
                        AcuiteVisuelPID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[ConsultOldPrescrs]
                      SET [ConsultationID] = @ConsultationID, [LAxis] = @LAxis, [LAddition] = @LAddition, [LIndex] = @LIndex, [LCylValue] = @LCylValue, [LSphValue] = @LSphValue, [RAxis] = @RAxis, [RAddition] = @RAddition, [RIndex] = @RIndex, [RCylValue] = @RCylValue, [RSphValue] = @RSphValue, [CategoryID] = @CategoryID, [DateDernierConsultation] = @DateDernierConsultation, [IsDilatation] = @IsDilatation, [IsCollyre] = @IsCollyre, [NomCollyre] = @NomCollyre, [PlaintePatient] = @PlaintePatient, [ConsultByID] = @ConsultByID, [DateConsultOldPres] = @DateConsultOldPres, [HeureConsOldPres] = @HeureConsOldPres, [CustomerNumber] = @CustomerNumber, [AcuiteVisuelLID] = @AcuiteVisuelLID, [AcuiteVisuelPID] = @AcuiteVisuelPID
                      WHERE ([ConsultOldPrescrID] = @ConsultOldPrescrID)"
            );
            
            CreateStoredProcedure(
                "dbo.ConsultOldPrescr_Delete",
                p => new
                    {
                        ConsultOldPrescrID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[ConsultOldPrescrs]
                      WHERE ([ConsultOldPrescrID] = @ConsultOldPrescrID)"
            );
            
            CreateStoredProcedure(
                "dbo.ConsultPrescrLastStep_Insert",
                p => new
                    {
                        ConsultationID = p.Int(),
                        DateNextConsultation = p.DateTime(),
                        IsCollyre = p.Boolean(),
                        NomCollyre = p.String(),
                        Remark = p.String(),
                        ConsultByID = p.Int(),
                        HeureConsLastStep = p.String(),
                        CustomerNumber = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[ConsultPrescrLastSteps]([ConsultationID], [DateNextConsultation], [IsCollyre], [NomCollyre], [Remark], [ConsultByID], [HeureConsLastStep], [CustomerNumber])
                      VALUES (@ConsultationID, @DateNextConsultation, @IsCollyre, @NomCollyre, @Remark, @ConsultByID, @HeureConsLastStep, @CustomerNumber)
                      
                      DECLARE @ConsultPrescrLastStepID int
                      SELECT @ConsultPrescrLastStepID = [ConsultPrescrLastStepID]
                      FROM [dbo].[ConsultPrescrLastSteps]
                      WHERE @@ROWCOUNT > 0 AND [ConsultPrescrLastStepID] = scope_identity()
                      
                      SELECT t0.[ConsultPrescrLastStepID]
                      FROM [dbo].[ConsultPrescrLastSteps] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[ConsultPrescrLastStepID] = @ConsultPrescrLastStepID"
            );
            
            CreateStoredProcedure(
                "dbo.ConsultPrescrLastStep_Update",
                p => new
                    {
                        ConsultPrescrLastStepID = p.Int(),
                        ConsultationID = p.Int(),
                        DateNextConsultation = p.DateTime(),
                        IsCollyre = p.Boolean(),
                        NomCollyre = p.String(),
                        Remark = p.String(),
                        ConsultByID = p.Int(),
                        HeureConsLastStep = p.String(),
                        CustomerNumber = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[ConsultPrescrLastSteps]
                      SET [ConsultationID] = @ConsultationID, [DateNextConsultation] = @DateNextConsultation, [IsCollyre] = @IsCollyre, [NomCollyre] = @NomCollyre, [Remark] = @Remark, [ConsultByID] = @ConsultByID, [HeureConsLastStep] = @HeureConsLastStep, [CustomerNumber] = @CustomerNumber
                      WHERE ([ConsultPrescrLastStepID] = @ConsultPrescrLastStepID)"
            );
            
            CreateStoredProcedure(
                "dbo.ConsultPrescrLastStep_Delete",
                p => new
                    {
                        ConsultPrescrLastStepID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[ConsultPrescrLastSteps]
                      WHERE ([ConsultPrescrLastStepID] = @ConsultPrescrLastStepID)"
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
                        ConsultDilPrescID = p.Int(),
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
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [ConsultDilPrescID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [CNI], [Consultation_ConsultationID])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @ConsultDilPrescID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @CNI, @Consultation_ConsultationID)
                      
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
                        ConsultDilPrescID = p.Int(),
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
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [ConsultDilPrescID] = @ConsultDilPrescID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [CNI] = @CNI, [Consultation_ConsultationID] = @Consultation_ConsultationID
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
                        ConsultDilPrescID = p.Int(),
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
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [ConsultDilPrescID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [CNI], [Consultation_ConsultationID])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @ConsultDilPrescID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @CNI, @Consultation_ConsultationID)
                      
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
                        ConsultDilPrescID = p.Int(),
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
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [ConsultDilPrescID] = @ConsultDilPrescID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [CNI] = @CNI, [Consultation_ConsultationID] = @Consultation_ConsultationID
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
                        ConsultDilPrescID = p.Int(),
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
                        BankRef = p.String(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [ConsultDilPrescID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [CNI], [Consultation_ConsultationID])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @ConsultDilPrescID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @CNI, @Consultation_ConsultationID)
                      
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
                        ConsultDilPrescID = p.Int(),
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
                        BankRef = p.String(),
                        Consultation_ConsultationID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[BankSales]
                      SET [BankID] = @BankID, [BankRef] = @BankRef
                      WHERE ([SaleID] = @SaleID)
                      
                      UPDATE [dbo].[Sales]
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [ConsultDilPrescID] = @ConsultDilPrescID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [CNI] = @CNI, [Consultation_ConsultationID] = @Consultation_ConsultationID
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
                        ConsultDilPrescID = p.Int(),
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
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [ConsultDilPrescID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [CNI], [Consultation_ConsultationID])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @ConsultDilPrescID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @CNI, @Consultation_ConsultationID)
                      
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
                        ConsultDilPrescID = p.Int(),
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
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [ConsultDilPrescID] = @ConsultDilPrescID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [CNI] = @CNI, [Consultation_ConsultationID] = @Consultation_ConsultationID
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
                        ConsultDilPrescID = p.Int(),
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
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [ConsultDilPrescID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [CNI], [Consultation_ConsultationID])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @ConsultDilPrescID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @CNI, @Consultation_ConsultationID)
                      
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
                        ConsultDilPrescID = p.Int(),
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
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [ConsultDilPrescID] = @ConsultDilPrescID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [CNI] = @CNI, [Consultation_ConsultationID] = @Consultation_ConsultationID
                      WHERE ([SaleID] = @SaleID)
                      
                      UPDATE [dbo].[TillSales]
                      SET [TillID] = @TillID
                      WHERE ([SaleID] = @SaleID)
                      AND @@ROWCOUNT > 0"
            );

            //initialise AVL And AVP Table with Data
            //AVL
            Sql(@"INSERT INTO dbo.AcuiteVisuelLs(Name) VALUES ('1/10')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelLs(Name) VALUES ('2/10')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelLs(Name) VALUES ('3/10')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelLs(Name) VALUES ('4/10')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelLs(Name) VALUES ('5/10')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelLs(Name) VALUES ('6/10')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelLs(Name) VALUES ('7/10')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelLs(Name) VALUES ('8/10')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelLs(Name) VALUES ('9/10')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelLs(Name) VALUES ('10/10')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelLs(Name) VALUES ('11/10')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelLs(Name) VALUES ('12/10')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelLs(Name) VALUES ('13/10')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelLs(Name) VALUES ('14/10')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelLs(Name) VALUES ('15/10')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelLs(Name) VALUES ('VBLM')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelLs(Name) VALUES ('CLD')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelLs(Name) VALUES ('PL+')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelLs(Name) VALUES ('PL-')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelLs(Name) VALUES ('TS')");
            //AVP
            Sql(@"INSERT INTO dbo.AcuiteVisuelPs(Name) VALUES ('P14')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelPs(Name) VALUES ('P12')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelPs(Name) VALUES ('P10')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelPs(Name) VALUES ('P8')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelPs(Name) VALUES ('P6')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelPs(Name) VALUES ('P4')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelPs(Name) VALUES ('P3')");
            Sql(@"INSERT INTO dbo.AcuiteVisuelPs(Name) VALUES ('P2')");
        }
        
        public override void Down()
        {
            DropStoredProcedure("dbo.ConsultPrescrLastStep_Delete");
            DropStoredProcedure("dbo.ConsultPrescrLastStep_Update");
            DropStoredProcedure("dbo.ConsultPrescrLastStep_Insert");
            DropStoredProcedure("dbo.ConsultOldPrescr_Delete");
            DropStoredProcedure("dbo.ConsultOldPrescr_Update");
            DropStoredProcedure("dbo.ConsultOldPrescr_Insert");
            DropStoredProcedure("dbo.ConsultLensPrescription_Delete");
            DropStoredProcedure("dbo.ConsultLensPrescription_Update");
            DropStoredProcedure("dbo.ConsultLensPrescription_Insert");
            DropStoredProcedure("dbo.ConsultDilatation_Delete");
            DropStoredProcedure("dbo.ConsultDilatation_Update");
            DropStoredProcedure("dbo.ConsultDilatation_Insert");
            DropStoredProcedure("dbo.AcuiteVisuelP_Delete");
            DropStoredProcedure("dbo.AcuiteVisuelP_Update");
            DropStoredProcedure("dbo.AcuiteVisuelP_Insert");
            DropStoredProcedure("dbo.AcuiteVisuelL_Delete");
            DropStoredProcedure("dbo.AcuiteVisuelL_Update");
            DropStoredProcedure("dbo.AcuiteVisuelL_Insert");
            DropStoredProcedure("dbo.ConsultDilPresc_Delete");
            DropStoredProcedure("dbo.ConsultDilPresc_Update");
            DropStoredProcedure("dbo.ConsultDilPresc_Insert");
            DropForeignKey("dbo.ConsultPrescrLastSteps", "ConsultByID", "dbo.Users");
            DropForeignKey("dbo.ConsultPrescrLastSteps", "ConsultationID", "dbo.Consultations");
            DropForeignKey("dbo.ConsultPersonalMedHistoes", "ConsultByID", "dbo.Users");
            DropForeignKey("dbo.ConsultPersonalMedHistoes", "ConsultationID", "dbo.Consultations");
            DropForeignKey("dbo.ConsultOldPrescrs", "ConsultByID", "dbo.Users");
            DropForeignKey("dbo.ConsultOldPrescrs", "ConsultationID", "dbo.Consultations");
            DropForeignKey("dbo.ConsultOldPrescrs", "CategoryID", "dbo.Categories");
            DropForeignKey("dbo.ConsultOldPrescrs", "AcuiteVisuelPID", "dbo.AcuiteVisuelPs");
            DropForeignKey("dbo.ConsultOldPrescrs", "AcuiteVisuelLID", "dbo.AcuiteVisuelLs");
            DropForeignKey("dbo.ConsultLensPrescriptions", "ConsultDilPrescID", "dbo.ConsultDilPrescs");
            DropForeignKey("dbo.ConsultLensPrescriptions", "ConsultByID", "dbo.Users");
            DropForeignKey("dbo.ConsultLensPrescriptions", "CategoryID", "dbo.Categories");
            DropForeignKey("dbo.ConsultDilatations", "ConsultDilPrescID", "dbo.ConsultDilPrescs");
            DropForeignKey("dbo.ConsultDilatations", "ConsultByID", "dbo.Users");
            DropForeignKey("dbo.Sales", "ConsultDilPrescID", "dbo.ConsultDilPrescs");
            DropForeignKey("dbo.AuthoriseSales", "ConsultDilPrescID", "dbo.ConsultDilPrescs");
            DropForeignKey("dbo.ConsultDilPrescs", "ConsultationID", "dbo.Consultations");
            DropIndex("dbo.ConsultPrescrLastSteps", new[] { "ConsultByID" });
            DropIndex("dbo.ConsultPrescrLastSteps", new[] { "ConsultationID" });
            DropIndex("dbo.ConsultPersonalMedHistoes", new[] { "ConsultByID" });
            DropIndex("dbo.ConsultPersonalMedHistoes", new[] { "ConsultationID" });
            DropIndex("dbo.ConsultOldPrescrs", new[] { "AcuiteVisuelPID" });
            DropIndex("dbo.ConsultOldPrescrs", new[] { "AcuiteVisuelLID" });
            DropIndex("dbo.ConsultOldPrescrs", new[] { "ConsultByID" });
            DropIndex("dbo.ConsultOldPrescrs", new[] { "CategoryID" });
            DropIndex("dbo.ConsultOldPrescrs", new[] { "ConsultationID" });
            DropIndex("dbo.ConsultLensPrescriptions", new[] { "ConsultByID" });
            DropIndex("dbo.ConsultLensPrescriptions", new[] { "CategoryID" });
            DropIndex("dbo.ConsultLensPrescriptions", new[] { "ConsultDilPrescID" });
            DropIndex("dbo.ConsultDilatations", new[] { "ConsultByID" });
            DropIndex("dbo.ConsultDilatations", new[] { "ConsultDilPrescID" });
            DropIndex("dbo.ConsultDilPrescs", new[] { "ConsultationID" });
            DropIndex("dbo.AuthoriseSales", new[] { "ConsultDilPrescID" });
            DropIndex("dbo.Sales", new[] { "ConsultDilPrescID" });
            DropColumn("dbo.AuthoriseSales", "ConsultDilPrescID");
            DropColumn("dbo.Sales", "ConsultDilPrescID");
            DropTable("dbo.ConsultPrescrLastSteps");
            DropTable("dbo.ConsultPersonalMedHistoes");
            DropTable("dbo.ConsultOldPrescrs");
            DropTable("dbo.ConsultLensPrescriptions");
            DropTable("dbo.ConsultDilatations");
            DropTable("dbo.AcuiteVisuelPs");
            DropTable("dbo.AcuiteVisuelLs");
            DropTable("dbo.ConsultDilPrescs");
            //throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
