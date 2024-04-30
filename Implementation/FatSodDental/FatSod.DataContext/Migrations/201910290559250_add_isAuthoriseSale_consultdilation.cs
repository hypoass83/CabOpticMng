namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_isAuthoriseSale_consultdilation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConsultDilatations", "isAuthoriseSale", c => c.Boolean(nullable: false));
            AlterStoredProcedure(
                "dbo.ConsultDilatation_Insert",
                p => new
                    {
                        ConsultDilPrescID = p.Int(),
                        CodeDilation = p.String(),
                        ConsultByID = p.Int(),
                        HeureConsultDilatation = p.String(),
                        DateDilation = p.DateTime(),
                        isAuthoriseSale = p.Boolean(),
                    },
                body:
                    @"INSERT [dbo].[ConsultDilatations]([ConsultDilPrescID], [CodeDilation], [ConsultByID], [HeureConsultDilatation], [DateDilation], [isAuthoriseSale])
                      VALUES (@ConsultDilPrescID, @CodeDilation, @ConsultByID, @HeureConsultDilatation, @DateDilation, @isAuthoriseSale)
                      
                      DECLARE @ConsultDilatationID int
                      SELECT @ConsultDilatationID = [ConsultDilatationID]
                      FROM [dbo].[ConsultDilatations]
                      WHERE @@ROWCOUNT > 0 AND [ConsultDilatationID] = scope_identity()
                      
                      SELECT t0.[ConsultDilatationID]
                      FROM [dbo].[ConsultDilatations] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[ConsultDilatationID] = @ConsultDilatationID"
            );
            
            AlterStoredProcedure(
                "dbo.ConsultDilatation_Update",
                p => new
                    {
                        ConsultDilatationID = p.Int(),
                        ConsultDilPrescID = p.Int(),
                        CodeDilation = p.String(),
                        ConsultByID = p.Int(),
                        HeureConsultDilatation = p.String(),
                        DateDilation = p.DateTime(),
                        isAuthoriseSale = p.Boolean(),
                    },
                body:
                    @"UPDATE [dbo].[ConsultDilatations]
                      SET [ConsultDilPrescID] = @ConsultDilPrescID, [CodeDilation] = @CodeDilation, [ConsultByID] = @ConsultByID, [HeureConsultDilatation] = @HeureConsultDilatation, [DateDilation] = @DateDilation, [isAuthoriseSale] = @isAuthoriseSale
                      WHERE ([ConsultDilatationID] = @ConsultDilatationID)"
            );
            
        }
        
        public override void Down()
        {
            DropColumn("dbo.ConsultDilatations", "isAuthoriseSale");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
