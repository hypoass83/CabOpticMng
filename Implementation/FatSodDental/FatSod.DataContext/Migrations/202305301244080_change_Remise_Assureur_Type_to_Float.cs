namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change_Remise_Assureur_Type_to_Float : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Assureurs", "Remise", c => c.Double(nullable: false));
            AlterStoredProcedure(
                "dbo.Assureur_Insert",
                p => new
                    {
                        Name = p.String(),
                        Tiergroup = p.String(),
                        Description = p.String(),
                        CNI = p.String(maxLength: 100),
                        AdressID = p.Int(),
                        IsConnected = p.Boolean(),
                        SexID = p.Int(),
                        IsMarketer = p.Boolean(),
                        IsSeller = p.Boolean(),
                        AccountID = p.Int(),
                        CompanyCapital = p.Int(),
                        CompanySigle = p.String(),
                        CompanyTradeRegister = p.String(),
                        CompanySlogan = p.String(),
                        CompanyIsDeletable = p.Boolean(),
                        CompteurFacture = p.Int(),
                        Matricule = p.Int(),
                        Remise = p.Double(),
                        Adress_AdressID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[GlobalPeople]([Name], [Tiergroup], [Description], [CNI], [AdressID])
                      VALUES (@Name, @Tiergroup, @Description, @CNI, @AdressID)
                      
                      DECLARE @GlobalPersonID int
                      SELECT @GlobalPersonID = [GlobalPersonID]
                      FROM [dbo].[GlobalPeople]
                      WHERE @@ROWCOUNT > 0 AND [GlobalPersonID] = scope_identity()
                      
                      INSERT [dbo].[People]([GlobalPersonID], [Adress_AdressID], [IsConnected], [SexID], [IsMarketer], [IsSeller])
                      VALUES (@GlobalPersonID, @Adress_AdressID, @IsConnected, @SexID, @IsMarketer, @IsSeller)
                      
                      INSERT [dbo].[Assureurs]([GlobalPersonID], [AccountID], [CompanyCapital], [CompanySigle], [CompanyTradeRegister], [CompanySlogan], [CompanyIsDeletable], [CompteurFacture], [Matricule], [Remise])
                      VALUES (@GlobalPersonID, @AccountID, @CompanyCapital, @CompanySigle, @CompanyTradeRegister, @CompanySlogan, @CompanyIsDeletable, @CompteurFacture, @Matricule, @Remise)
                      
                      SELECT t0.[GlobalPersonID]
                      FROM [dbo].[GlobalPeople] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[GlobalPersonID] = @GlobalPersonID"
            );
            
            AlterStoredProcedure(
                "dbo.Assureur_Update",
                p => new
                    {
                        GlobalPersonID = p.Int(),
                        Name = p.String(),
                        Tiergroup = p.String(),
                        Description = p.String(),
                        CNI = p.String(maxLength: 100),
                        AdressID = p.Int(),
                        IsConnected = p.Boolean(),
                        SexID = p.Int(),
                        IsMarketer = p.Boolean(),
                        IsSeller = p.Boolean(),
                        AccountID = p.Int(),
                        CompanyCapital = p.Int(),
                        CompanySigle = p.String(),
                        CompanyTradeRegister = p.String(),
                        CompanySlogan = p.String(),
                        CompanyIsDeletable = p.Boolean(),
                        CompteurFacture = p.Int(),
                        Matricule = p.Int(),
                        Remise = p.Double(),
                        Adress_AdressID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Assureurs]
                      SET [AccountID] = @AccountID, [CompanyCapital] = @CompanyCapital, [CompanySigle] = @CompanySigle, [CompanyTradeRegister] = @CompanyTradeRegister, [CompanySlogan] = @CompanySlogan, [CompanyIsDeletable] = @CompanyIsDeletable, [CompteurFacture] = @CompteurFacture, [Matricule] = @Matricule, [Remise] = @Remise
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      
                      UPDATE [dbo].[GlobalPeople]
                      SET [Name] = @Name, [Tiergroup] = @Tiergroup, [Description] = @Description, [CNI] = @CNI, [AdressID] = @AdressID
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0
                      
                      UPDATE [dbo].[People]
                      SET [Adress_AdressID] = @Adress_AdressID, [IsConnected] = @IsConnected, [SexID] = @SexID, [IsMarketer] = @IsMarketer, [IsSeller] = @IsSeller
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0"
            );
            
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Assureurs", "Remise", c => c.Int(nullable: false));
            //throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
