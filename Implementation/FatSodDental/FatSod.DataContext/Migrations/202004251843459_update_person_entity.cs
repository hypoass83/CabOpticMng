namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_person_entity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "IsMarketer", c => c.Boolean(nullable: false));
            AddColumn("dbo.People", "IsSeller", c => c.Boolean(nullable: false));
            AlterStoredProcedure(
                "dbo.User_Insert",
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
                        Code = p.String(maxLength: 100),
                        UserLogin = p.String(maxLength: 100),
                        UserPassword = p.String(),
                        UserAccountState = p.Boolean(),
                        UserAccessLevel = p.Int(),
                        ProfileID = p.Int(),
                        UserConfigurationID = p.Int(),
                        JobID = p.Int(),
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
                      
                      INSERT [dbo].[Users]([GlobalPersonID], [Code], [UserLogin], [UserPassword], [UserAccountState], [UserAccessLevel], [ProfileID], [UserConfigurationID], [JobID])
                      VALUES (@GlobalPersonID, @Code, @UserLogin, @UserPassword, @UserAccountState, @UserAccessLevel, @ProfileID, @UserConfigurationID, @JobID)
                      
                      SELECT t0.[GlobalPersonID]
                      FROM [dbo].[GlobalPeople] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[GlobalPersonID] = @GlobalPersonID"
            );
            
            AlterStoredProcedure(
                "dbo.User_Update",
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
                        Code = p.String(maxLength: 100),
                        UserLogin = p.String(maxLength: 100),
                        UserPassword = p.String(),
                        UserAccountState = p.Boolean(),
                        UserAccessLevel = p.Int(),
                        ProfileID = p.Int(),
                        UserConfigurationID = p.Int(),
                        JobID = p.Int(),
                        Adress_AdressID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[GlobalPeople]
                      SET [Name] = @Name, [Tiergroup] = @Tiergroup, [Description] = @Description, [CNI] = @CNI, [AdressID] = @AdressID
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      
                      UPDATE [dbo].[People]
                      SET [Adress_AdressID] = @Adress_AdressID, [IsConnected] = @IsConnected, [SexID] = @SexID, [IsMarketer] = @IsMarketer, [IsSeller] = @IsSeller
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0
                      
                      UPDATE [dbo].[Users]
                      SET [Code] = @Code, [UserLogin] = @UserLogin, [UserPassword] = @UserPassword, [UserAccountState] = @UserAccountState, [UserAccessLevel] = @UserAccessLevel, [ProfileID] = @ProfileID, [UserConfigurationID] = @UserConfigurationID, [JobID] = @JobID
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0"
            );
            
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
                      
                      INSERT [dbo].[Assureurs]([GlobalPersonID], [AccountID], [CompanyCapital], [CompanySigle], [CompanyTradeRegister], [CompanySlogan], [CompanyIsDeletable], [CompteurFacture], [Matricule])
                      VALUES (@GlobalPersonID, @AccountID, @CompanyCapital, @CompanySigle, @CompanyTradeRegister, @CompanySlogan, @CompanyIsDeletable, @CompteurFacture, @Matricule)
                      
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
                        Adress_AdressID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Assureurs]
                      SET [AccountID] = @AccountID, [CompanyCapital] = @CompanyCapital, [CompanySigle] = @CompanySigle, [CompanyTradeRegister] = @CompanyTradeRegister, [CompanySlogan] = @CompanySlogan, [CompanyIsDeletable] = @CompanyIsDeletable, [CompteurFacture] = @CompteurFacture, [Matricule] = @Matricule
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
                      
                      INSERT [dbo].[Customers]([GlobalPersonID], [AccountID], [IsCashCustomer], [PoliceAssurance], [CompanyName], [DateOfBirth], [IsBillCustomer], [CustomerNumber], [Profession], [AssureurID], [GestionnaireID], [LimitAmount], [Dateregister])
                      VALUES (@GlobalPersonID, @AccountID, @IsCashCustomer, @PoliceAssurance, @CompanyName, @DateOfBirth, @IsBillCustomer, @CustomerNumber, @Profession, @AssureurID, @GestionnaireID, @LimitAmount, @Dateregister)
                      
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
                      SET [AccountID] = @AccountID, [IsCashCustomer] = @IsCashCustomer, [PoliceAssurance] = @PoliceAssurance, [CompanyName] = @CompanyName, [DateOfBirth] = @DateOfBirth, [IsBillCustomer] = @IsBillCustomer, [CustomerNumber] = @CustomerNumber, [Profession] = @Profession, [AssureurID] = @AssureurID, [GestionnaireID] = @GestionnaireID, [LimitAmount] = @LimitAmount, [Dateregister] = @Dateregister
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
            
            AlterStoredProcedure(
                "dbo.Supplier_Insert",
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
                        SupplierNumber = p.String(maxLength: 250),
                        CompanyCapital = p.Int(),
                        CompanySigle = p.String(),
                        CompanyTradeRegister = p.String(),
                        CompanySlogan = p.String(),
                        CompanyIsDeletable = p.Boolean(),
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
                      
                      INSERT [dbo].[Suppliers]([GlobalPersonID], [AccountID], [SupplierNumber], [CompanyCapital], [CompanySigle], [CompanyTradeRegister], [CompanySlogan], [CompanyIsDeletable])
                      VALUES (@GlobalPersonID, @AccountID, @SupplierNumber, @CompanyCapital, @CompanySigle, @CompanyTradeRegister, @CompanySlogan, @CompanyIsDeletable)
                      
                      SELECT t0.[GlobalPersonID]
                      FROM [dbo].[GlobalPeople] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[GlobalPersonID] = @GlobalPersonID"
            );
            
            AlterStoredProcedure(
                "dbo.Supplier_Update",
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
                        SupplierNumber = p.String(maxLength: 250),
                        CompanyCapital = p.Int(),
                        CompanySigle = p.String(),
                        CompanyTradeRegister = p.String(),
                        CompanySlogan = p.String(),
                        CompanyIsDeletable = p.Boolean(),
                        Adress_AdressID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[GlobalPeople]
                      SET [Name] = @Name, [Tiergroup] = @Tiergroup, [Description] = @Description, [CNI] = @CNI, [AdressID] = @AdressID
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      
                      UPDATE [dbo].[People]
                      SET [Adress_AdressID] = @Adress_AdressID, [IsConnected] = @IsConnected, [SexID] = @SexID, [IsMarketer] = @IsMarketer, [IsSeller] = @IsSeller
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0
                      
                      UPDATE [dbo].[Suppliers]
                      SET [AccountID] = @AccountID, [SupplierNumber] = @SupplierNumber, [CompanyCapital] = @CompanyCapital, [CompanySigle] = @CompanySigle, [CompanyTradeRegister] = @CompanyTradeRegister, [CompanySlogan] = @CompanySlogan, [CompanyIsDeletable] = @CompanyIsDeletable
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0"
            );

            //update de la bd pour metre les user par defaut marketeur et vendeur
            Sql(@"update p 
            set IsMarketer=1,IsSeller=1
            FROM [ValdozInventory].[dbo].[Users] u inner join ValdozInventory.dbo.People p on u.GlobalPersonID=p.GlobalPersonID
            inner join ValdozInventory.dbo.GlobalPeople g on g.GlobalPersonID=u.GlobalPersonID
            where ProfileID>2 and IsConnected=1");
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "IsSeller");
            DropColumn("dbo.People", "IsMarketer");
            //throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
