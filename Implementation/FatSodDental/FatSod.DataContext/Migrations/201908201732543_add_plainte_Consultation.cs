namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_plainte_Consultation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Consultations", "plainte", c => c.String());
            AlterStoredProcedure(
                "dbo.Consultation_Insert",
                p => new
                    {
                        CustomerID = p.Int(),
                        IsNewCustomer = p.Boolean(),
                        isPrescritionValidate = p.Boolean(),
                        plainte = p.String(),
                        DateConsultation = p.DateTime(),
                    },
                body:
                    @"INSERT [dbo].[Consultations]([CustomerID], [IsNewCustomer], [isPrescritionValidate], [plainte], [DateConsultation])
                      VALUES (@CustomerID, @IsNewCustomer, @isPrescritionValidate, @plainte, @DateConsultation)
                      
                      DECLARE @ConsultationID int
                      SELECT @ConsultationID = [ConsultationID]
                      FROM [dbo].[Consultations]
                      WHERE @@ROWCOUNT > 0 AND [ConsultationID] = scope_identity()
                      
                      SELECT t0.[ConsultationID]
                      FROM [dbo].[Consultations] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[ConsultationID] = @ConsultationID"
            );
            
            AlterStoredProcedure(
                "dbo.Consultation_Update",
                p => new
                    {
                        ConsultationID = p.Int(),
                        CustomerID = p.Int(),
                        IsNewCustomer = p.Boolean(),
                        isPrescritionValidate = p.Boolean(),
                        plainte = p.String(),
                        DateConsultation = p.DateTime(),
                    },
                body:
                    @"UPDATE [dbo].[Consultations]
                      SET [CustomerID] = @CustomerID, [IsNewCustomer] = @IsNewCustomer, [isPrescritionValidate] = @isPrescritionValidate, [plainte] = @plainte, [DateConsultation] = @DateConsultation
                      WHERE ([ConsultationID] = @ConsultationID)"
            );
            
        }
        
        public override void Down()
        {
            DropColumn("dbo.Consultations", "plainte");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
