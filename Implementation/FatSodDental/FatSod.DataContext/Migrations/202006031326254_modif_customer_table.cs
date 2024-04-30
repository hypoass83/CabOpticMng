namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class modif_customer_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "IsInHouseCustomer", c => c.Boolean(nullable: false));
            DropColumn("dbo.Customers", "IsCashCustomer");
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
                    IsInHouseCustomer = p.Boolean(),
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
                      
                      INSERT [dbo].[Customers]([GlobalPersonID], [AccountID], [IsInHouseCustomer], [CustomerValue], [LastCustomerValue], [PoliceAssurance], [CompanyName], [DateOfBirth], [IsBillCustomer], [CustomerNumber], [Profession], [AssureurID], [GestionnaireID], [LimitAmount], [Dateregister])
                      VALUES (@GlobalPersonID, @AccountID, @IsInHouseCustomer, @CustomerValue, @LastCustomerValue, @PoliceAssurance, @CompanyName, @DateOfBirth, @IsBillCustomer, @CustomerNumber, @Profession, @AssureurID, @GestionnaireID, @LimitAmount, @Dateregister)
                      
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
                    IsInHouseCustomer = p.Boolean(),
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
                      SET [AccountID] = @AccountID, [IsInHouseCustomer] = @IsInHouseCustomer, [CustomerValue] = @CustomerValue, [LastCustomerValue] = @LastCustomerValue, [PoliceAssurance] = @PoliceAssurance, [CompanyName] = @CompanyName, [DateOfBirth] = @DateOfBirth, [IsBillCustomer] = @IsBillCustomer, [CustomerNumber] = @CustomerNumber, [Profession] = @Profession, [AssureurID] = @AssureurID, [GestionnaireID] = @GestionnaireID, [LimitAmount] = @LimitAmount, [Dateregister] = @Dateregister
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
            //update table and set all customers by default inhouse customer
            Sql(@"update c set IsInHouseCustomer= case when (select count(*) FROM [ValdozInventory].[dbo].customers u inner join ValdozInventory.dbo.GlobalPeople g on u.GlobalPersonID=g.GlobalPersonID
			where u.Dateregister>='2020-04-04' and (len(g.CNI)>=9 or isnumeric(g.CNI)=0) and u.GlobalPersonID=c.GlobalPersonID)>=1 then 0 else 1 end
			FROM [ValdozInventory].[dbo].customers c");
            //alter view customer to add is inhouse field
            Sql(@"Alter VIEW [dbo].[viewCustomers] AS SELECT NEWID() AS Id, g.GlobalPersonID, g.Name, g.Description, g.CNI, c.CustomerNumber, c.Dateregister, a.AdressPOBox, a.AdressEmail, a.AdressPhoneNumber, q.QuarterLabel, a.AdressID, q.QuarterCode, q.QuarterID, 
                         c.IsBillCustomer,c.IsInHouseCustomer, c.AccountID, ac.AccountNumber, ac.AccountLabel, p.SexID, s.SexCode, s.SexLabel,
						 CASE WHEN ISNULL(CustomerValue,0)=0 THEN 'ECO' ELSE 'VIP' END AS CustomerValue,
						 CASE WHEN ISNULL(LastCustomerValue,0)=0 THEN 'ECO' ELSE 'VIP' END AS LastCustomerValue
                         FROM dbo.GlobalPeople AS g INNER JOIN
                         dbo.People AS p ON g.GlobalPersonID = p.GlobalPersonID INNER JOIN
                         dbo.Customers AS c ON c.GlobalPersonID = p.GlobalPersonID INNER JOIN
                         dbo.Adresses AS a ON g.AdressID = a.AdressID INNER JOIN
                         dbo.Quarters AS q ON a.QuarterID = q.QuarterID INNER JOIN
                         dbo.Accounts AS ac ON c.AccountID = ac.AccountID INNER JOIN
                         dbo.Sexes AS s ON p.SexID = s.SexID");
        }

        public override void Down()
        {
            AddColumn("dbo.Customers", "IsCashCustomer", c => c.Int(nullable: false));
            DropColumn("dbo.Customers", "IsInHouseCustomer");

            Sql(@"Alter VIEW [dbo].[viewCustomers] AS SELECT NEWID() AS Id, g.GlobalPersonID, g.Name, g.Description, g.CNI, c.CustomerNumber, c.Dateregister, a.AdressPOBox, a.AdressEmail, a.AdressPhoneNumber, q.QuarterLabel, a.AdressID, q.QuarterCode, q.QuarterID, 
                         c.IsBillCustomer, c.AccountID, ac.AccountNumber, ac.AccountLabel, p.SexID, s.SexCode, s.SexLabel,
						 CASE WHEN ISNULL(CustomerValue,0)=0 THEN 'ECO' ELSE 'VIP' END AS CustomerValue,
						 CASE WHEN ISNULL(LastCustomerValue,0)=0 THEN 'ECO' ELSE 'VIP' END AS LastCustomerValue
                         FROM dbo.GlobalPeople AS g INNER JOIN
                         dbo.People AS p ON g.GlobalPersonID = p.GlobalPersonID INNER JOIN
                         dbo.Customers AS c ON c.GlobalPersonID = p.GlobalPersonID INNER JOIN
                         dbo.Adresses AS a ON g.AdressID = a.AdressID INNER JOIN
                         dbo.Quarters AS q ON a.QuarterID = q.QuarterID INNER JOIN
                         dbo.Accounts AS ac ON c.AccountID = ac.AccountID INNER JOIN
                         dbo.Sexes AS s ON p.SexID = s.SexID");
        }
    }
}
