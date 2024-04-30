namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Consultations_RaisonRdv : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Consultations", "RaisonRdv", c => c.String());
            AlterStoredProcedure(
                "dbo.Consultation_Insert",
                p => new
                    {
                        CustomerID = p.Int(),
                        IsNewCustomer = p.Boolean(),
                        isPrescritionValidate = p.Boolean(),
                        MedecintTraitant = p.String(),
                        RaisonRdv = p.String(),
                        DateConsultation = p.DateTime(),
                    },
                body:
                    @"INSERT [dbo].[Consultations]([CustomerID], [IsNewCustomer], [isPrescritionValidate], [MedecintTraitant], [RaisonRdv], [DateConsultation])
                      VALUES (@CustomerID, @IsNewCustomer, @isPrescritionValidate, @MedecintTraitant, @RaisonRdv, @DateConsultation)
                      
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
                        MedecintTraitant = p.String(),
                        RaisonRdv = p.String(),
                        DateConsultation = p.DateTime(),
                    },
                body:
                    @"UPDATE [dbo].[Consultations]
                      SET [CustomerID] = @CustomerID, [IsNewCustomer] = @IsNewCustomer, [isPrescritionValidate] = @isPrescritionValidate, [MedecintTraitant] = @MedecintTraitant, [RaisonRdv] = @RaisonRdv, [DateConsultation] = @DateConsultation
                      WHERE ([ConsultationID] = @ConsultationID)"
            );
            
        }
        
        public override void Down()
        {
            DropColumn("dbo.Consultations", "RaisonRdv");
            //throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
