namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modif_consuldilpres_medecin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConsultLensPrescriptions", "isAuthoriseSale", c => c.Boolean(nullable: false));
            AlterStoredProcedure(
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
                        isAuthoriseSale = p.Boolean(),
                    },
                body:
                    @"INSERT [dbo].[ConsultLensPrescriptions]([ConsultDilPrescID], [LAxis], [LAddition], [LIndex], [LCylValue], [LSphValue], [RAxis], [RAddition], [RIndex], [RCylValue], [RSphValue], [CategoryID], [SupplyingName], [DatePrescription], [ConsultByID], [HeureLensPrescription], [isAuthoriseSale])
                      VALUES (@ConsultDilPrescID, @LAxis, @LAddition, @LIndex, @LCylValue, @LSphValue, @RAxis, @RAddition, @RIndex, @RCylValue, @RSphValue, @CategoryID, @SupplyingName, @DatePrescription, @ConsultByID, @HeureLensPrescription, @isAuthoriseSale)
                      
                      DECLARE @ConsultLensPrescriptionID int
                      SELECT @ConsultLensPrescriptionID = [ConsultLensPrescriptionID]
                      FROM [dbo].[ConsultLensPrescriptions]
                      WHERE @@ROWCOUNT > 0 AND [ConsultLensPrescriptionID] = scope_identity()
                      
                      SELECT t0.[ConsultLensPrescriptionID]
                      FROM [dbo].[ConsultLensPrescriptions] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[ConsultLensPrescriptionID] = @ConsultLensPrescriptionID"
            );
            
            AlterStoredProcedure(
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
                        isAuthoriseSale = p.Boolean(),
                    },
                body:
                    @"UPDATE [dbo].[ConsultLensPrescriptions]
                      SET [ConsultDilPrescID] = @ConsultDilPrescID, [LAxis] = @LAxis, [LAddition] = @LAddition, [LIndex] = @LIndex, [LCylValue] = @LCylValue, [LSphValue] = @LSphValue, [RAxis] = @RAxis, [RAddition] = @RAddition, [RIndex] = @RIndex, [RCylValue] = @RCylValue, [RSphValue] = @RSphValue, [CategoryID] = @CategoryID, [SupplyingName] = @SupplyingName, [DatePrescription] = @DatePrescription, [ConsultByID] = @ConsultByID, [HeureLensPrescription] = @HeureLensPrescription, [isAuthoriseSale] = @isAuthoriseSale
                      WHERE ([ConsultLensPrescriptionID] = @ConsultLensPrescriptionID)"
            );
            
        }
        
        public override void Down()
        {
            DropColumn("dbo.ConsultLensPrescriptions", "isAuthoriseSale");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
