namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddONOCNumberToCompanies : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "ONOCNumber", c => c.String());
            AlterStoredProcedure(
                "dbo.Company_Insert",
                p => new
                    {
                        Name = p.String(),
                        Tiergroup = p.String(),
                        Description = p.String(),
                        CNI = p.String(maxLength: 100),
                        AdressID = p.Int(),
                        CompanyCapital = p.Int(),
                        CompanySigle = p.String(maxLength: 50),
                        CompanyTradeRegister = p.String(maxLength: 50),
                        ONOCNumber = p.String(),
                        CompanySlogan = p.String(),
                        CompanyIsDeletable = p.Boolean(),
                    },
                body:
                    @"INSERT [dbo].[GlobalPeople]([Name], [Tiergroup], [Description], [CNI], [AdressID])
                      VALUES (@Name, @Tiergroup, @Description, @CNI, @AdressID)
                      
                      DECLARE @GlobalPersonID int
                      SELECT @GlobalPersonID = [GlobalPersonID]
                      FROM [dbo].[GlobalPeople]
                      WHERE @@ROWCOUNT > 0 AND [GlobalPersonID] = scope_identity()
                      
                      INSERT [dbo].[Companies]([GlobalPersonID], [CompanyCapital], [CompanySigle], [CompanyTradeRegister], [ONOCNumber], [CompanySlogan], [CompanyIsDeletable])
                      VALUES (@GlobalPersonID, @CompanyCapital, @CompanySigle, @CompanyTradeRegister, @ONOCNumber, @CompanySlogan, @CompanyIsDeletable)
                      
                      SELECT t0.[GlobalPersonID]
                      FROM [dbo].[GlobalPeople] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[GlobalPersonID] = @GlobalPersonID"
            );
            
            AlterStoredProcedure(
                "dbo.Company_Update",
                p => new
                    {
                        GlobalPersonID = p.Int(),
                        Name = p.String(),
                        Tiergroup = p.String(),
                        Description = p.String(),
                        CNI = p.String(maxLength: 100),
                        AdressID = p.Int(),
                        CompanyCapital = p.Int(),
                        CompanySigle = p.String(maxLength: 50),
                        CompanyTradeRegister = p.String(maxLength: 50),
                        ONOCNumber = p.String(),
                        CompanySlogan = p.String(),
                        CompanyIsDeletable = p.Boolean(),
                    },
                body:
                    @"UPDATE [dbo].[Companies]
                      SET [CompanyCapital] = @CompanyCapital, [CompanySigle] = @CompanySigle, [CompanyTradeRegister] = @CompanyTradeRegister, [ONOCNumber] = @ONOCNumber, [CompanySlogan] = @CompanySlogan, [CompanyIsDeletable] = @CompanyIsDeletable
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      
                      UPDATE [dbo].[GlobalPeople]
                      SET [Name] = @Name, [Tiergroup] = @Tiergroup, [Description] = @Description, [CNI] = @CNI, [AdressID] = @AdressID
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0"
            );
            
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "ONOCNumber");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
