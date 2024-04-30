namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class manage_customer_value : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "CustomerValue", c => c.Int(nullable: false, defaultValue: 0));
            AddColumn("dbo.Customers", "LastCustomerValue", c => c.Int(nullable: false, defaultValue: 0));
            AlterStoredProcedure(
                "dbo.Customer_Insert",
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
                        IsCashCustomer = p.Int(),
                        CustomerValue = p.Int(),
                        LastCustomerValue = p.Int(),
                        PoliceAssurance = p.String(maxLength: 250),
                        CompanyName = p.String(maxLength: 250),
                        DateOfBirth = p.DateTime(),
                        IsBillCustomer = p.Boolean(),
                        CustomerNumber = p.String(maxLength: 10),
                        Profession = p.String(maxLength: 250),
                        AssureurID = p.Int(),
                        GestionnaireID = p.Int(),
                        LimitAmount = p.Double(),
                        Dateregister = p.DateTime(),
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
                      
                      INSERT [dbo].[Customers]([GlobalPersonID], [AccountID], [IsCashCustomer], [CustomerValue], [LastCustomerValue], [PoliceAssurance], [CompanyName], [DateOfBirth], [IsBillCustomer], [CustomerNumber], [Profession], [AssureurID], [GestionnaireID], [LimitAmount], [Dateregister])
                      VALUES (@GlobalPersonID, @AccountID, @IsCashCustomer, @CustomerValue, @LastCustomerValue, @PoliceAssurance, @CompanyName, @DateOfBirth, @IsBillCustomer, @CustomerNumber, @Profession, @AssureurID, @GestionnaireID, @LimitAmount, @Dateregister)
                      
                      SELECT t0.[GlobalPersonID]
                      FROM [dbo].[GlobalPeople] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[GlobalPersonID] = @GlobalPersonID"
            );
            
            AlterStoredProcedure(
                "dbo.Customer_Update",
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
                        IsCashCustomer = p.Int(),
                        CustomerValue = p.Int(),
                        LastCustomerValue = p.Int(),
                        PoliceAssurance = p.String(maxLength: 250),
                        CompanyName = p.String(maxLength: 250),
                        DateOfBirth = p.DateTime(),
                        IsBillCustomer = p.Boolean(),
                        CustomerNumber = p.String(maxLength: 10),
                        Profession = p.String(maxLength: 250),
                        AssureurID = p.Int(),
                        GestionnaireID = p.Int(),
                        LimitAmount = p.Double(),
                        Dateregister = p.DateTime(),
                        Adress_AdressID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Customers]
                      SET [AccountID] = @AccountID, [IsCashCustomer] = @IsCashCustomer, [CustomerValue] = @CustomerValue, [LastCustomerValue] = @LastCustomerValue, [PoliceAssurance] = @PoliceAssurance, [CompanyName] = @CompanyName, [DateOfBirth] = @DateOfBirth, [IsBillCustomer] = @IsBillCustomer, [CustomerNumber] = @CustomerNumber, [Profession] = @Profession, [AssureurID] = @AssureurID, [GestionnaireID] = @GestionnaireID, [LimitAmount] = @LimitAmount, [Dateregister] = @Dateregister
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
            DropColumn("dbo.Customers", "LastCustomerValue");
            DropColumn("dbo.Customers", "CustomerValue");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
