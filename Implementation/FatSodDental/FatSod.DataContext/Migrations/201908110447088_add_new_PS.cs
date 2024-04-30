namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_new_PS : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(
                "dbo.AccountOperation_Insert",
                p => new
                    {
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[AccountOperations]([BranchID], [OperationID], [AccountID], [AccountTierID], [DeviseID], [DateOperation], [Description], [Reference], [CodeTransaction], [Debit], [Credit], [Account_AccountID])
                      VALUES (@BranchID, @OperationID, @AccountID, @AccountTierID, @DeviseID, @DateOperation, @Description, @Reference, @CodeTransaction, @Debit, @Credit, @Account_AccountID)
                      
                      DECLARE @AccountOperationID bigint
                      SELECT @AccountOperationID = [AccountOperationID]
                      FROM [dbo].[AccountOperations]
                      WHERE @@ROWCOUNT > 0 AND [AccountOperationID] = scope_identity()
                      
                      SELECT t0.[AccountOperationID]
                      FROM [dbo].[AccountOperations] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[AccountOperationID] = @AccountOperationID"
            );
            
            CreateStoredProcedure(
                "dbo.AccountOperation_Update",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[AccountOperations]
                      SET [BranchID] = @BranchID, [OperationID] = @OperationID, [AccountID] = @AccountID, [AccountTierID] = @AccountTierID, [DeviseID] = @DeviseID, [DateOperation] = @DateOperation, [Description] = @Description, [Reference] = @Reference, [CodeTransaction] = @CodeTransaction, [Debit] = @Debit, [Credit] = @Credit, [Account_AccountID] = @Account_AccountID
                      WHERE ([AccountOperationID] = @AccountOperationID)"
            );
            
            CreateStoredProcedure(
                "dbo.AccountOperation_Delete",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[AccountOperations]
                      WHERE (([AccountOperationID] = @AccountOperationID) AND (([Account_AccountID] = @Account_AccountID) OR ([Account_AccountID] IS NULL AND @Account_AccountID IS NULL)))"
            );
            
            CreateStoredProcedure(
                "dbo.DepositAccountOperation_Insert",
                p => new
                    {
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        DepositID = p.Int(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[AccountOperations]([BranchID], [OperationID], [AccountID], [AccountTierID], [DeviseID], [DateOperation], [Description], [Reference], [CodeTransaction], [Debit], [Credit], [Account_AccountID])
                      VALUES (@BranchID, @OperationID, @AccountID, @AccountTierID, @DeviseID, @DateOperation, @Description, @Reference, @CodeTransaction, @Debit, @Credit, @Account_AccountID)
                      
                      DECLARE @AccountOperationID bigint
                      SELECT @AccountOperationID = [AccountOperationID]
                      FROM [dbo].[AccountOperations]
                      WHERE @@ROWCOUNT > 0 AND [AccountOperationID] = scope_identity()
                      
                      INSERT [dbo].[DepositAccountOperations]([AccountOperationID], [DepositID])
                      VALUES (@AccountOperationID, @DepositID)
                      
                      SELECT t0.[AccountOperationID]
                      FROM [dbo].[AccountOperations] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[AccountOperationID] = @AccountOperationID"
            );
            
            CreateStoredProcedure(
                "dbo.DepositAccountOperation_Update",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        DepositID = p.Int(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[AccountOperations]
                      SET [BranchID] = @BranchID, [OperationID] = @OperationID, [AccountID] = @AccountID, [AccountTierID] = @AccountTierID, [DeviseID] = @DeviseID, [DateOperation] = @DateOperation, [Description] = @Description, [Reference] = @Reference, [CodeTransaction] = @CodeTransaction, [Debit] = @Debit, [Credit] = @Credit, [Account_AccountID] = @Account_AccountID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      UPDATE [dbo].[DepositAccountOperations]
                      SET [DepositID] = @DepositID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.DepositAccountOperation_Delete",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[DepositAccountOperations]
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      DELETE [dbo].[AccountOperations]
                      WHERE (([AccountOperationID] = @AccountOperationID) AND (([Account_AccountID] = @Account_AccountID) OR ([Account_AccountID] IS NULL AND @Account_AccountID IS NULL)))
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.ManualAccountOperation_Insert",
                p => new
                    {
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        PieceID = p.Long(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[AccountOperations]([BranchID], [OperationID], [AccountID], [AccountTierID], [DeviseID], [DateOperation], [Description], [Reference], [CodeTransaction], [Debit], [Credit], [Account_AccountID])
                      VALUES (@BranchID, @OperationID, @AccountID, @AccountTierID, @DeviseID, @DateOperation, @Description, @Reference, @CodeTransaction, @Debit, @Credit, @Account_AccountID)
                      
                      DECLARE @AccountOperationID bigint
                      SELECT @AccountOperationID = [AccountOperationID]
                      FROM [dbo].[AccountOperations]
                      WHERE @@ROWCOUNT > 0 AND [AccountOperationID] = scope_identity()
                      
                      INSERT [dbo].[ManualAccountOperations]([AccountOperationID], [PieceID])
                      VALUES (@AccountOperationID, @PieceID)
                      
                      SELECT t0.[AccountOperationID]
                      FROM [dbo].[AccountOperations] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[AccountOperationID] = @AccountOperationID"
            );
            
            CreateStoredProcedure(
                "dbo.ManualAccountOperation_Update",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        PieceID = p.Long(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[AccountOperations]
                      SET [BranchID] = @BranchID, [OperationID] = @OperationID, [AccountID] = @AccountID, [AccountTierID] = @AccountTierID, [DeviseID] = @DeviseID, [DateOperation] = @DateOperation, [Description] = @Description, [Reference] = @Reference, [CodeTransaction] = @CodeTransaction, [Debit] = @Debit, [Credit] = @Credit, [Account_AccountID] = @Account_AccountID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      UPDATE [dbo].[ManualAccountOperations]
                      SET [PieceID] = @PieceID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.ManualAccountOperation_Delete",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[ManualAccountOperations]
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      DELETE [dbo].[AccountOperations]
                      WHERE (([AccountOperationID] = @AccountOperationID) AND (([Account_AccountID] = @Account_AccountID) OR ([Account_AccountID] IS NULL AND @Account_AccountID IS NULL)))
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.SaleReturnAccountOperation_Insert",
                p => new
                    {
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        CustomerReturnID = p.Int(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[AccountOperations]([BranchID], [OperationID], [AccountID], [AccountTierID], [DeviseID], [DateOperation], [Description], [Reference], [CodeTransaction], [Debit], [Credit], [Account_AccountID])
                      VALUES (@BranchID, @OperationID, @AccountID, @AccountTierID, @DeviseID, @DateOperation, @Description, @Reference, @CodeTransaction, @Debit, @Credit, @Account_AccountID)
                      
                      DECLARE @AccountOperationID bigint
                      SELECT @AccountOperationID = [AccountOperationID]
                      FROM [dbo].[AccountOperations]
                      WHERE @@ROWCOUNT > 0 AND [AccountOperationID] = scope_identity()
                      
                      INSERT [dbo].[SaleReturnAccountOperations]([AccountOperationID], [CustomerReturnID])
                      VALUES (@AccountOperationID, @CustomerReturnID)
                      
                      SELECT t0.[AccountOperationID]
                      FROM [dbo].[AccountOperations] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[AccountOperationID] = @AccountOperationID"
            );
            
            CreateStoredProcedure(
                "dbo.SaleReturnAccountOperation_Update",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        CustomerReturnID = p.Int(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[AccountOperations]
                      SET [BranchID] = @BranchID, [OperationID] = @OperationID, [AccountID] = @AccountID, [AccountTierID] = @AccountTierID, [DeviseID] = @DeviseID, [DateOperation] = @DateOperation, [Description] = @Description, [Reference] = @Reference, [CodeTransaction] = @CodeTransaction, [Debit] = @Debit, [Credit] = @Credit, [Account_AccountID] = @Account_AccountID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      UPDATE [dbo].[SaleReturnAccountOperations]
                      SET [CustomerReturnID] = @CustomerReturnID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.SaleReturnAccountOperation_Delete",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[SaleReturnAccountOperations]
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      DELETE [dbo].[AccountOperations]
                      WHERE (([AccountOperationID] = @AccountOperationID) AND (([Account_AccountID] = @Account_AccountID) OR ([Account_AccountID] IS NULL AND @Account_AccountID IS NULL)))
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.SaleAccountOperation_Insert",
                p => new
                    {
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        SaleID = p.Int(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[AccountOperations]([BranchID], [OperationID], [AccountID], [AccountTierID], [DeviseID], [DateOperation], [Description], [Reference], [CodeTransaction], [Debit], [Credit], [Account_AccountID])
                      VALUES (@BranchID, @OperationID, @AccountID, @AccountTierID, @DeviseID, @DateOperation, @Description, @Reference, @CodeTransaction, @Debit, @Credit, @Account_AccountID)
                      
                      DECLARE @AccountOperationID bigint
                      SELECT @AccountOperationID = [AccountOperationID]
                      FROM [dbo].[AccountOperations]
                      WHERE @@ROWCOUNT > 0 AND [AccountOperationID] = scope_identity()
                      
                      INSERT [dbo].[SaleAccountOperations]([AccountOperationID], [SaleID])
                      VALUES (@AccountOperationID, @SaleID)
                      
                      SELECT t0.[AccountOperationID]
                      FROM [dbo].[AccountOperations] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[AccountOperationID] = @AccountOperationID"
            );
            
            CreateStoredProcedure(
                "dbo.SaleAccountOperation_Update",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        SaleID = p.Int(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[AccountOperations]
                      SET [BranchID] = @BranchID, [OperationID] = @OperationID, [AccountID] = @AccountID, [AccountTierID] = @AccountTierID, [DeviseID] = @DeviseID, [DateOperation] = @DateOperation, [Description] = @Description, [Reference] = @Reference, [CodeTransaction] = @CodeTransaction, [Debit] = @Debit, [Credit] = @Credit, [Account_AccountID] = @Account_AccountID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      UPDATE [dbo].[SaleAccountOperations]
                      SET [SaleID] = @SaleID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.SaleAccountOperation_Delete",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[SaleAccountOperations]
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      DELETE [dbo].[AccountOperations]
                      WHERE (([AccountOperationID] = @AccountOperationID) AND (([Account_AccountID] = @Account_AccountID) OR ([Account_AccountID] IS NULL AND @Account_AccountID IS NULL)))
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.BudgetConsumptionAccountOperation_Insert",
                p => new
                    {
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        BudgetConsumptionID = p.Int(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[AccountOperations]([BranchID], [OperationID], [AccountID], [AccountTierID], [DeviseID], [DateOperation], [Description], [Reference], [CodeTransaction], [Debit], [Credit], [Account_AccountID])
                      VALUES (@BranchID, @OperationID, @AccountID, @AccountTierID, @DeviseID, @DateOperation, @Description, @Reference, @CodeTransaction, @Debit, @Credit, @Account_AccountID)
                      
                      DECLARE @AccountOperationID bigint
                      SELECT @AccountOperationID = [AccountOperationID]
                      FROM [dbo].[AccountOperations]
                      WHERE @@ROWCOUNT > 0 AND [AccountOperationID] = scope_identity()
                      
                      INSERT [dbo].[BudgetConsumptionAccountOperations]([AccountOperationID], [BudgetConsumptionID])
                      VALUES (@AccountOperationID, @BudgetConsumptionID)
                      
                      SELECT t0.[AccountOperationID]
                      FROM [dbo].[AccountOperations] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[AccountOperationID] = @AccountOperationID"
            );
            
            CreateStoredProcedure(
                "dbo.BudgetConsumptionAccountOperation_Update",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        BudgetConsumptionID = p.Int(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[AccountOperations]
                      SET [BranchID] = @BranchID, [OperationID] = @OperationID, [AccountID] = @AccountID, [AccountTierID] = @AccountTierID, [DeviseID] = @DeviseID, [DateOperation] = @DateOperation, [Description] = @Description, [Reference] = @Reference, [CodeTransaction] = @CodeTransaction, [Debit] = @Debit, [Credit] = @Credit, [Account_AccountID] = @Account_AccountID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      UPDATE [dbo].[BudgetConsumptionAccountOperations]
                      SET [BudgetConsumptionID] = @BudgetConsumptionID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.BudgetConsumptionAccountOperation_Delete",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[BudgetConsumptionAccountOperations]
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      DELETE [dbo].[AccountOperations]
                      WHERE (([AccountOperationID] = @AccountOperationID) AND (([Account_AccountID] = @Account_AccountID) OR ([Account_AccountID] IS NULL AND @Account_AccountID IS NULL)))
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.ProductLocalizationAccountOperation_Insert",
                p => new
                    {
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        ProductLocalizationID = p.Int(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[AccountOperations]([BranchID], [OperationID], [AccountID], [AccountTierID], [DeviseID], [DateOperation], [Description], [Reference], [CodeTransaction], [Debit], [Credit], [Account_AccountID])
                      VALUES (@BranchID, @OperationID, @AccountID, @AccountTierID, @DeviseID, @DateOperation, @Description, @Reference, @CodeTransaction, @Debit, @Credit, @Account_AccountID)
                      
                      DECLARE @AccountOperationID bigint
                      SELECT @AccountOperationID = [AccountOperationID]
                      FROM [dbo].[AccountOperations]
                      WHERE @@ROWCOUNT > 0 AND [AccountOperationID] = scope_identity()
                      
                      INSERT [dbo].[ProductLocalizationAccountOperations]([AccountOperationID], [ProductLocalizationID])
                      VALUES (@AccountOperationID, @ProductLocalizationID)
                      
                      SELECT t0.[AccountOperationID]
                      FROM [dbo].[AccountOperations] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[AccountOperationID] = @AccountOperationID"
            );
            
            CreateStoredProcedure(
                "dbo.ProductLocalizationAccountOperation_Update",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        ProductLocalizationID = p.Int(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[AccountOperations]
                      SET [BranchID] = @BranchID, [OperationID] = @OperationID, [AccountID] = @AccountID, [AccountTierID] = @AccountTierID, [DeviseID] = @DeviseID, [DateOperation] = @DateOperation, [Description] = @Description, [Reference] = @Reference, [CodeTransaction] = @CodeTransaction, [Debit] = @Debit, [Credit] = @Credit, [Account_AccountID] = @Account_AccountID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      UPDATE [dbo].[ProductLocalizationAccountOperations]
                      SET [ProductLocalizationID] = @ProductLocalizationID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.ProductLocalizationAccountOperation_Delete",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[ProductLocalizationAccountOperations]
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      DELETE [dbo].[AccountOperations]
                      WHERE (([AccountOperationID] = @AccountOperationID) AND (([Account_AccountID] = @Account_AccountID) OR ([Account_AccountID] IS NULL AND @Account_AccountID IS NULL)))
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.ProductTransferAccountOperation_Insert",
                p => new
                    {
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        ProductTransfertID = p.Int(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[AccountOperations]([BranchID], [OperationID], [AccountID], [AccountTierID], [DeviseID], [DateOperation], [Description], [Reference], [CodeTransaction], [Debit], [Credit], [Account_AccountID])
                      VALUES (@BranchID, @OperationID, @AccountID, @AccountTierID, @DeviseID, @DateOperation, @Description, @Reference, @CodeTransaction, @Debit, @Credit, @Account_AccountID)
                      
                      DECLARE @AccountOperationID bigint
                      SELECT @AccountOperationID = [AccountOperationID]
                      FROM [dbo].[AccountOperations]
                      WHERE @@ROWCOUNT > 0 AND [AccountOperationID] = scope_identity()
                      
                      INSERT [dbo].[ProductTransferAccountOperations]([AccountOperationID], [ProductTransfertID])
                      VALUES (@AccountOperationID, @ProductTransfertID)
                      
                      SELECT t0.[AccountOperationID]
                      FROM [dbo].[AccountOperations] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[AccountOperationID] = @AccountOperationID"
            );
            
            CreateStoredProcedure(
                "dbo.ProductTransferAccountOperation_Update",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        ProductTransfertID = p.Int(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[AccountOperations]
                      SET [BranchID] = @BranchID, [OperationID] = @OperationID, [AccountID] = @AccountID, [AccountTierID] = @AccountTierID, [DeviseID] = @DeviseID, [DateOperation] = @DateOperation, [Description] = @Description, [Reference] = @Reference, [CodeTransaction] = @CodeTransaction, [Debit] = @Debit, [Credit] = @Credit, [Account_AccountID] = @Account_AccountID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      UPDATE [dbo].[ProductTransferAccountOperations]
                      SET [ProductTransfertID] = @ProductTransfertID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.ProductTransferAccountOperation_Delete",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[ProductTransferAccountOperations]
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      DELETE [dbo].[AccountOperations]
                      WHERE (([AccountOperationID] = @AccountOperationID) AND (([Account_AccountID] = @Account_AccountID) OR ([Account_AccountID] IS NULL AND @Account_AccountID IS NULL)))
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.PurchaseAccountOperation_Insert",
                p => new
                    {
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        PurchaseID = p.Int(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[AccountOperations]([BranchID], [OperationID], [AccountID], [AccountTierID], [DeviseID], [DateOperation], [Description], [Reference], [CodeTransaction], [Debit], [Credit], [Account_AccountID])
                      VALUES (@BranchID, @OperationID, @AccountID, @AccountTierID, @DeviseID, @DateOperation, @Description, @Reference, @CodeTransaction, @Debit, @Credit, @Account_AccountID)
                      
                      DECLARE @AccountOperationID bigint
                      SELECT @AccountOperationID = [AccountOperationID]
                      FROM [dbo].[AccountOperations]
                      WHERE @@ROWCOUNT > 0 AND [AccountOperationID] = scope_identity()
                      
                      INSERT [dbo].[PurchaseAccountOperations]([AccountOperationID], [PurchaseID])
                      VALUES (@AccountOperationID, @PurchaseID)
                      
                      SELECT t0.[AccountOperationID]
                      FROM [dbo].[AccountOperations] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[AccountOperationID] = @AccountOperationID"
            );
            
            CreateStoredProcedure(
                "dbo.PurchaseAccountOperation_Update",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        PurchaseID = p.Int(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[AccountOperations]
                      SET [BranchID] = @BranchID, [OperationID] = @OperationID, [AccountID] = @AccountID, [AccountTierID] = @AccountTierID, [DeviseID] = @DeviseID, [DateOperation] = @DateOperation, [Description] = @Description, [Reference] = @Reference, [CodeTransaction] = @CodeTransaction, [Debit] = @Debit, [Credit] = @Credit, [Account_AccountID] = @Account_AccountID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      UPDATE [dbo].[PurchaseAccountOperations]
                      SET [PurchaseID] = @PurchaseID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.PurchaseAccountOperation_Delete",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[PurchaseAccountOperations]
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      DELETE [dbo].[AccountOperations]
                      WHERE (([AccountOperationID] = @AccountOperationID) AND (([Account_AccountID] = @Account_AccountID) OR ([Account_AccountID] IS NULL AND @Account_AccountID IS NULL)))
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.PurchaseReturnAccountOperation_Insert",
                p => new
                    {
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        SupplierReturnID = p.Int(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[AccountOperations]([BranchID], [OperationID], [AccountID], [AccountTierID], [DeviseID], [DateOperation], [Description], [Reference], [CodeTransaction], [Debit], [Credit], [Account_AccountID])
                      VALUES (@BranchID, @OperationID, @AccountID, @AccountTierID, @DeviseID, @DateOperation, @Description, @Reference, @CodeTransaction, @Debit, @Credit, @Account_AccountID)
                      
                      DECLARE @AccountOperationID bigint
                      SELECT @AccountOperationID = [AccountOperationID]
                      FROM [dbo].[AccountOperations]
                      WHERE @@ROWCOUNT > 0 AND [AccountOperationID] = scope_identity()
                      
                      INSERT [dbo].[PurchaseReturnAccountOperations]([AccountOperationID], [SupplierReturnID])
                      VALUES (@AccountOperationID, @SupplierReturnID)
                      
                      SELECT t0.[AccountOperationID]
                      FROM [dbo].[AccountOperations] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[AccountOperationID] = @AccountOperationID"
            );
            
            CreateStoredProcedure(
                "dbo.PurchaseReturnAccountOperation_Update",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        SupplierReturnID = p.Int(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[AccountOperations]
                      SET [BranchID] = @BranchID, [OperationID] = @OperationID, [AccountID] = @AccountID, [AccountTierID] = @AccountTierID, [DeviseID] = @DeviseID, [DateOperation] = @DateOperation, [Description] = @Description, [Reference] = @Reference, [CodeTransaction] = @CodeTransaction, [Debit] = @Debit, [Credit] = @Credit, [Account_AccountID] = @Account_AccountID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      UPDATE [dbo].[PurchaseReturnAccountOperations]
                      SET [SupplierReturnID] = @SupplierReturnID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.PurchaseReturnAccountOperation_Delete",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[PurchaseReturnAccountOperations]
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      DELETE [dbo].[AccountOperations]
                      WHERE (([AccountOperationID] = @AccountOperationID) AND (([Account_AccountID] = @Account_AccountID) OR ([Account_AccountID] IS NULL AND @Account_AccountID IS NULL)))
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.TillAdjustAccountOperation_Insert",
                p => new
                    {
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        TillAdjustID = p.Int(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[AccountOperations]([BranchID], [OperationID], [AccountID], [AccountTierID], [DeviseID], [DateOperation], [Description], [Reference], [CodeTransaction], [Debit], [Credit], [Account_AccountID])
                      VALUES (@BranchID, @OperationID, @AccountID, @AccountTierID, @DeviseID, @DateOperation, @Description, @Reference, @CodeTransaction, @Debit, @Credit, @Account_AccountID)
                      
                      DECLARE @AccountOperationID bigint
                      SELECT @AccountOperationID = [AccountOperationID]
                      FROM [dbo].[AccountOperations]
                      WHERE @@ROWCOUNT > 0 AND [AccountOperationID] = scope_identity()
                      
                      INSERT [dbo].[TillAdjustAccountOperations]([AccountOperationID], [TillAdjustID])
                      VALUES (@AccountOperationID, @TillAdjustID)
                      
                      SELECT t0.[AccountOperationID]
                      FROM [dbo].[AccountOperations] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[AccountOperationID] = @AccountOperationID"
            );
            
            CreateStoredProcedure(
                "dbo.TillAdjustAccountOperation_Update",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        TillAdjustID = p.Int(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[AccountOperations]
                      SET [BranchID] = @BranchID, [OperationID] = @OperationID, [AccountID] = @AccountID, [AccountTierID] = @AccountTierID, [DeviseID] = @DeviseID, [DateOperation] = @DateOperation, [Description] = @Description, [Reference] = @Reference, [CodeTransaction] = @CodeTransaction, [Debit] = @Debit, [Credit] = @Credit, [Account_AccountID] = @Account_AccountID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      UPDATE [dbo].[TillAdjustAccountOperations]
                      SET [TillAdjustID] = @TillAdjustID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.TillAdjustAccountOperation_Delete",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[TillAdjustAccountOperations]
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      DELETE [dbo].[AccountOperations]
                      WHERE (([AccountOperationID] = @AccountOperationID) AND (([Account_AccountID] = @Account_AccountID) OR ([Account_AccountID] IS NULL AND @Account_AccountID IS NULL)))
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.TreasuryOperationAccountOperation_Insert",
                p => new
                    {
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        TreasuryOperationID = p.Int(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[AccountOperations]([BranchID], [OperationID], [AccountID], [AccountTierID], [DeviseID], [DateOperation], [Description], [Reference], [CodeTransaction], [Debit], [Credit], [Account_AccountID])
                      VALUES (@BranchID, @OperationID, @AccountID, @AccountTierID, @DeviseID, @DateOperation, @Description, @Reference, @CodeTransaction, @Debit, @Credit, @Account_AccountID)
                      
                      DECLARE @AccountOperationID bigint
                      SELECT @AccountOperationID = [AccountOperationID]
                      FROM [dbo].[AccountOperations]
                      WHERE @@ROWCOUNT > 0 AND [AccountOperationID] = scope_identity()
                      
                      INSERT [dbo].[TreasuryOperationAccountOperations]([AccountOperationID], [TreasuryOperationID])
                      VALUES (@AccountOperationID, @TreasuryOperationID)
                      
                      SELECT t0.[AccountOperationID]
                      FROM [dbo].[AccountOperations] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[AccountOperationID] = @AccountOperationID"
            );
            
            CreateStoredProcedure(
                "dbo.TreasuryOperationAccountOperation_Update",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        BranchID = p.Int(),
                        OperationID = p.Int(),
                        AccountID = p.Int(),
                        AccountTierID = p.Int(),
                        DeviseID = p.Int(),
                        DateOperation = p.DateTime(),
                        Description = p.String(),
                        Reference = p.String(),
                        CodeTransaction = p.String(),
                        Debit = p.Double(),
                        Credit = p.Double(),
                        TreasuryOperationID = p.Int(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[AccountOperations]
                      SET [BranchID] = @BranchID, [OperationID] = @OperationID, [AccountID] = @AccountID, [AccountTierID] = @AccountTierID, [DeviseID] = @DeviseID, [DateOperation] = @DateOperation, [Description] = @Description, [Reference] = @Reference, [CodeTransaction] = @CodeTransaction, [Debit] = @Debit, [Credit] = @Credit, [Account_AccountID] = @Account_AccountID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      UPDATE [dbo].[TreasuryOperationAccountOperations]
                      SET [TreasuryOperationID] = @TreasuryOperationID
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.TreasuryOperationAccountOperation_Delete",
                p => new
                    {
                        AccountOperationID = p.Long(),
                        Account_AccountID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[TreasuryOperationAccountOperations]
                      WHERE ([AccountOperationID] = @AccountOperationID)
                      
                      DELETE [dbo].[AccountOperations]
                      WHERE (([AccountOperationID] = @AccountOperationID) AND (([Account_AccountID] = @Account_AccountID) OR ([Account_AccountID] IS NULL AND @Account_AccountID IS NULL)))
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
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
                      
                      INSERT [dbo].[Companies]([GlobalPersonID], [CompanyCapital], [CompanySigle], [CompanyTradeRegister], [CompanySlogan], [CompanyIsDeletable])
                      VALUES (@GlobalPersonID, @CompanyCapital, @CompanySigle, @CompanyTradeRegister, @CompanySlogan, @CompanyIsDeletable)
                      
                      SELECT t0.[GlobalPersonID]
                      FROM [dbo].[GlobalPeople] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[GlobalPersonID] = @GlobalPersonID"
            );
            
            CreateStoredProcedure(
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
                        CompanySlogan = p.String(),
                        CompanyIsDeletable = p.Boolean(),
                    },
                body:
                    @"UPDATE [dbo].[Companies]
                      SET [CompanyCapital] = @CompanyCapital, [CompanySigle] = @CompanySigle, [CompanyTradeRegister] = @CompanyTradeRegister, [CompanySlogan] = @CompanySlogan, [CompanyIsDeletable] = @CompanyIsDeletable
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      
                      UPDATE [dbo].[GlobalPeople]
                      SET [Name] = @Name, [Tiergroup] = @Tiergroup, [Description] = @Description, [CNI] = @CNI, [AdressID] = @AdressID
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.Company_Delete",
                p => new
                    {
                        GlobalPersonID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Companies]
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      
                      DELETE [dbo].[GlobalPeople]
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
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
                      
                      INSERT [dbo].[People]([GlobalPersonID], [Adress_AdressID], [IsConnected], [SexID])
                      VALUES (@GlobalPersonID, @Adress_AdressID, @IsConnected, @SexID)
                      
                      INSERT [dbo].[Users]([GlobalPersonID], [Code], [UserLogin], [UserPassword], [UserAccountState], [UserAccessLevel], [ProfileID], [UserConfigurationID], [JobID])
                      VALUES (@GlobalPersonID, @Code, @UserLogin, @UserPassword, @UserAccountState, @UserAccessLevel, @ProfileID, @UserConfigurationID, @JobID)
                      
                      SELECT t0.[GlobalPersonID]
                      FROM [dbo].[GlobalPeople] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[GlobalPersonID] = @GlobalPersonID"
            );
            
            CreateStoredProcedure(
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
                      SET [Adress_AdressID] = @Adress_AdressID, [IsConnected] = @IsConnected, [SexID] = @SexID
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0
                      
                      UPDATE [dbo].[Users]
                      SET [Code] = @Code, [UserLogin] = @UserLogin, [UserPassword] = @UserPassword, [UserAccountState] = @UserAccountState, [UserAccessLevel] = @UserAccessLevel, [ProfileID] = @ProfileID, [UserConfigurationID] = @UserConfigurationID, [JobID] = @JobID
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.User_Delete",
                p => new
                    {
                        GlobalPersonID = p.Int(),
                        Adress_AdressID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Users]
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      
                      DELETE [dbo].[People]
                      WHERE (([GlobalPersonID] = @GlobalPersonID) AND (([Adress_AdressID] = @Adress_AdressID) OR ([Adress_AdressID] IS NULL AND @Adress_AdressID IS NULL)))
                      AND @@ROWCOUNT > 0
                      
                      DELETE [dbo].[GlobalPeople]
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
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
                      
                      INSERT [dbo].[People]([GlobalPersonID], [Adress_AdressID], [IsConnected], [SexID])
                      VALUES (@GlobalPersonID, @Adress_AdressID, @IsConnected, @SexID)
                      
                      INSERT [dbo].[Assureurs]([GlobalPersonID], [AccountID], [CompanyCapital], [CompanySigle], [CompanyTradeRegister], [CompanySlogan], [CompanyIsDeletable], [CompteurFacture], [Matricule])
                      VALUES (@GlobalPersonID, @AccountID, @CompanyCapital, @CompanySigle, @CompanyTradeRegister, @CompanySlogan, @CompanyIsDeletable, @CompteurFacture, @Matricule)
                      
                      SELECT t0.[GlobalPersonID]
                      FROM [dbo].[GlobalPeople] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[GlobalPersonID] = @GlobalPersonID"
            );
            
            CreateStoredProcedure(
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
                      SET [Adress_AdressID] = @Adress_AdressID, [IsConnected] = @IsConnected, [SexID] = @SexID
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.Assureur_Delete",
                p => new
                    {
                        GlobalPersonID = p.Int(),
                        Adress_AdressID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Assureurs]
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      
                      DELETE [dbo].[People]
                      WHERE (([GlobalPersonID] = @GlobalPersonID) AND (([Adress_AdressID] = @Adress_AdressID) OR ([Adress_AdressID] IS NULL AND @Adress_AdressID IS NULL)))
                      AND @@ROWCOUNT > 0
                      
                      DELETE [dbo].[GlobalPeople]
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
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
                        AccountID = p.Int(),
                        IsCashCustomer = p.Int(),
                        PoliceAssurance = p.String(maxLength: 250),
                        CompanyName = p.String(maxLength: 250),
                        DateOfBirth = p.DateTime(),
                        IsBillCustomer = p.Boolean(),
                        CustomerNumber = p.String(maxLength: 10),
                        isPrescritionValidate = p.Boolean(),
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
                      
                      INSERT [dbo].[People]([GlobalPersonID], [Adress_AdressID], [IsConnected], [SexID])
                      VALUES (@GlobalPersonID, @Adress_AdressID, @IsConnected, @SexID)
                      
                      INSERT [dbo].[Customers]([GlobalPersonID], [AccountID], [IsCashCustomer], [PoliceAssurance], [CompanyName], [DateOfBirth], [IsBillCustomer], [CustomerNumber], [isPrescritionValidate], [Profession], [AssureurID], [GestionnaireID], [LimitAmount], [Dateregister])
                      VALUES (@GlobalPersonID, @AccountID, @IsCashCustomer, @PoliceAssurance, @CompanyName, @DateOfBirth, @IsBillCustomer, @CustomerNumber, @isPrescritionValidate, @Profession, @AssureurID, @GestionnaireID, @LimitAmount, @Dateregister)
                      
                      SELECT t0.[GlobalPersonID]
                      FROM [dbo].[GlobalPeople] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[GlobalPersonID] = @GlobalPersonID"
            );
            
            CreateStoredProcedure(
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
                        AccountID = p.Int(),
                        IsCashCustomer = p.Int(),
                        PoliceAssurance = p.String(maxLength: 250),
                        CompanyName = p.String(maxLength: 250),
                        DateOfBirth = p.DateTime(),
                        IsBillCustomer = p.Boolean(),
                        CustomerNumber = p.String(maxLength: 10),
                        isPrescritionValidate = p.Boolean(),
                        Profession = p.String(maxLength: 250),
                        AssureurID = p.Int(),
                        GestionnaireID = p.Int(),
                        LimitAmount = p.Double(),
                        Dateregister = p.DateTime(),
                        Adress_AdressID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Customers]
                      SET [AccountID] = @AccountID, [IsCashCustomer] = @IsCashCustomer, [PoliceAssurance] = @PoliceAssurance, [CompanyName] = @CompanyName, [DateOfBirth] = @DateOfBirth, [IsBillCustomer] = @IsBillCustomer, [CustomerNumber] = @CustomerNumber, [isPrescritionValidate] = @isPrescritionValidate, [Profession] = @Profession, [AssureurID] = @AssureurID, [GestionnaireID] = @GestionnaireID, [LimitAmount] = @LimitAmount, [Dateregister] = @Dateregister
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      
                      UPDATE [dbo].[GlobalPeople]
                      SET [Name] = @Name, [Tiergroup] = @Tiergroup, [Description] = @Description, [CNI] = @CNI, [AdressID] = @AdressID
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0
                      
                      UPDATE [dbo].[People]
                      SET [Adress_AdressID] = @Adress_AdressID, [IsConnected] = @IsConnected, [SexID] = @SexID
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.Customer_Delete",
                p => new
                    {
                        GlobalPersonID = p.Int(),
                        Adress_AdressID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Customers]
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      
                      DELETE [dbo].[People]
                      WHERE (([GlobalPersonID] = @GlobalPersonID) AND (([Adress_AdressID] = @Adress_AdressID) OR ([Adress_AdressID] IS NULL AND @Adress_AdressID IS NULL)))
                      AND @@ROWCOUNT > 0
                      
                      DELETE [dbo].[GlobalPeople]
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
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
                      
                      INSERT [dbo].[People]([GlobalPersonID], [Adress_AdressID], [IsConnected], [SexID])
                      VALUES (@GlobalPersonID, @Adress_AdressID, @IsConnected, @SexID)
                      
                      INSERT [dbo].[Suppliers]([GlobalPersonID], [AccountID], [SupplierNumber], [CompanyCapital], [CompanySigle], [CompanyTradeRegister], [CompanySlogan], [CompanyIsDeletable])
                      VALUES (@GlobalPersonID, @AccountID, @SupplierNumber, @CompanyCapital, @CompanySigle, @CompanyTradeRegister, @CompanySlogan, @CompanyIsDeletable)
                      
                      SELECT t0.[GlobalPersonID]
                      FROM [dbo].[GlobalPeople] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[GlobalPersonID] = @GlobalPersonID"
            );
            
            CreateStoredProcedure(
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
                      SET [Adress_AdressID] = @Adress_AdressID, [IsConnected] = @IsConnected, [SexID] = @SexID
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0
                      
                      UPDATE [dbo].[Suppliers]
                      SET [AccountID] = @AccountID, [SupplierNumber] = @SupplierNumber, [CompanyCapital] = @CompanyCapital, [CompanySigle] = @CompanySigle, [CompanyTradeRegister] = @CompanyTradeRegister, [CompanySlogan] = @CompanySlogan, [CompanyIsDeletable] = @CompanyIsDeletable
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.Supplier_Delete",
                p => new
                    {
                        GlobalPersonID = p.Int(),
                        Adress_AdressID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Suppliers]
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      
                      DELETE [dbo].[People]
                      WHERE (([GlobalPersonID] = @GlobalPersonID) AND (([Adress_AdressID] = @Adress_AdressID) OR ([Adress_AdressID] IS NULL AND @Adress_AdressID IS NULL)))
                      AND @@ROWCOUNT > 0
                      
                      DELETE [dbo].[GlobalPeople]
                      WHERE ([GlobalPersonID] = @GlobalPersonID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
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
                    },
                body:
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [CNI])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @CNI)
                      
                      DECLARE @SaleID int
                      SELECT @SaleID = [SaleID]
                      FROM [dbo].[Sales]
                      WHERE @@ROWCOUNT > 0 AND [SaleID] = scope_identity()
                      
                      SELECT t0.[SaleID]
                      FROM [dbo].[Sales] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[SaleID] = @SaleID"
            );
            
            CreateStoredProcedure(
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
                    },
                body:
                    @"UPDATE [dbo].[Sales]
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [CNI] = @CNI
                      WHERE ([SaleID] = @SaleID)"
            );
            
            CreateStoredProcedure(
                "dbo.Sale_Delete",
                p => new
                    {
                        SaleID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Sales]
                      WHERE ([SaleID] = @SaleID)"
            );
            
            CreateStoredProcedure(
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
                    },
                body:
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [CNI])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @CNI)
                      
                      DECLARE @SaleID int
                      SELECT @SaleID = [SaleID]
                      FROM [dbo].[Sales]
                      WHERE @@ROWCOUNT > 0 AND [SaleID] = scope_identity()
                      
                      INSERT [dbo].[BankSales]([SaleID], [BankID])
                      VALUES (@SaleID, @BankID)
                      
                      SELECT t0.[SaleID]
                      FROM [dbo].[Sales] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[SaleID] = @SaleID"
            );
            
            CreateStoredProcedure(
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
                    },
                body:
                    @"UPDATE [dbo].[BankSales]
                      SET [BankID] = @BankID
                      WHERE ([SaleID] = @SaleID)
                      
                      UPDATE [dbo].[Sales]
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [CNI] = @CNI
                      WHERE ([SaleID] = @SaleID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.BankSale_Delete",
                p => new
                    {
                        SaleID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[BankSales]
                      WHERE ([SaleID] = @SaleID)
                      
                      DELETE [dbo].[Sales]
                      WHERE ([SaleID] = @SaleID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
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
                    },
                body:
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [CNI])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @CNI)
                      
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
            
            CreateStoredProcedure(
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
                    },
                body:
                    @"UPDATE [dbo].[Sales]
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [CNI] = @CNI
                      WHERE ([SaleID] = @SaleID)
                      
                      UPDATE [dbo].[TillSales]
                      SET [TillID] = @TillID
                      WHERE ([SaleID] = @SaleID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.TillSale_Delete",
                p => new
                    {
                        SaleID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[TillSales]
                      WHERE ([SaleID] = @SaleID)
                      
                      DELETE [dbo].[Sales]
                      WHERE ([SaleID] = @SaleID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
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
                    },
                body:
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [CNI])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @CNI)
                      
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
            
            CreateStoredProcedure(
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
                    },
                body:
                    @"UPDATE [dbo].[AssureurSales]
                      SET [AssureurPMID] = @AssureurPMID
                      WHERE ([SaleID] = @SaleID)
                      
                      UPDATE [dbo].[Sales]
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [CNI] = @CNI
                      WHERE ([SaleID] = @SaleID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.AssureurSale_Delete",
                p => new
                    {
                        SaleID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[AssureurSales]
                      WHERE ([SaleID] = @SaleID)
                      
                      DELETE [dbo].[Sales]
                      WHERE ([SaleID] = @SaleID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
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
                    },
                body:
                    @"INSERT [dbo].[Sales]([CompteurFacture], [SaleDeliver], [VatRate], [RateReduction], [RateDiscount], [Transport], [SaleDeliveryDate], [SaleDate], [SaleDateHours], [SaleValidate], [PoliceAssurance], [PaymentDelay], [Guaranteed], [Patient], [DeviseID], [IsPaid], [SaleReceiptNumber], [BranchID], [CustomerID], [CustomerName], [isReturn], [StatutSale], [OperatorID], [PostByID], [IsSpecialOrder], [Remarque], [MedecinTraitant], [CustomerOrderID], [AuthoriseSaleID], [DateRdv], [IsRendezVous], [AwaitingDay], [IsProductReveive], [EffectiveReceiveDate], [IsCustomerRceive], [CustomerDeliverDate], [PostSOByID], [ReceiveSOByID], [CNI])
                      VALUES (@CompteurFacture, @SaleDeliver, @VatRate, @RateReduction, @RateDiscount, @Transport, @SaleDeliveryDate, @SaleDate, @SaleDateHours, @SaleValidate, @PoliceAssurance, @PaymentDelay, @Guaranteed, @Patient, @DeviseID, @IsPaid, @SaleReceiptNumber, @BranchID, @CustomerID, @CustomerName, @isReturn, @StatutSale, @OperatorID, @PostByID, @IsSpecialOrder, @Remarque, @MedecinTraitant, @CustomerOrderID, @AuthoriseSaleID, @DateRdv, @IsRendezVous, @AwaitingDay, @IsProductReveive, @EffectiveReceiveDate, @IsCustomerRceive, @CustomerDeliverDate, @PostSOByID, @ReceiveSOByID, @CNI)
                      
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
            
            CreateStoredProcedure(
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
                    },
                body:
                    @"UPDATE [dbo].[Sales]
                      SET [CompteurFacture] = @CompteurFacture, [SaleDeliver] = @SaleDeliver, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Transport] = @Transport, [SaleDeliveryDate] = @SaleDeliveryDate, [SaleDate] = @SaleDate, [SaleDateHours] = @SaleDateHours, [SaleValidate] = @SaleValidate, [PoliceAssurance] = @PoliceAssurance, [PaymentDelay] = @PaymentDelay, [Guaranteed] = @Guaranteed, [Patient] = @Patient, [DeviseID] = @DeviseID, [IsPaid] = @IsPaid, [SaleReceiptNumber] = @SaleReceiptNumber, [BranchID] = @BranchID, [CustomerID] = @CustomerID, [CustomerName] = @CustomerName, [isReturn] = @isReturn, [StatutSale] = @StatutSale, [OperatorID] = @OperatorID, [PostByID] = @PostByID, [IsSpecialOrder] = @IsSpecialOrder, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [CustomerOrderID] = @CustomerOrderID, [AuthoriseSaleID] = @AuthoriseSaleID, [DateRdv] = @DateRdv, [IsRendezVous] = @IsRendezVous, [AwaitingDay] = @AwaitingDay, [IsProductReveive] = @IsProductReveive, [EffectiveReceiveDate] = @EffectiveReceiveDate, [IsCustomerRceive] = @IsCustomerRceive, [CustomerDeliverDate] = @CustomerDeliverDate, [PostSOByID] = @PostSOByID, [ReceiveSOByID] = @ReceiveSOByID, [CNI] = @CNI
                      WHERE ([SaleID] = @SaleID)
                      
                      UPDATE [dbo].[SavingAccountSales]
                      SET [SavingAccountID] = @SavingAccountID
                      WHERE ([SaleID] = @SaleID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.SavingAccountSale_Delete",
                p => new
                    {
                        SaleID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[SavingAccountSales]
                      WHERE ([SaleID] = @SaleID)
                      
                      DELETE [dbo].[Sales]
                      WHERE ([SaleID] = @SaleID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.AuthoriseSaleLine_Insert",
                p => new
                    {
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        AuthoriseSaleID = p.Int(),
                        SpecialOrderLineCode = p.String(),
                        marque = p.String(),
                        reference = p.String(),
                        Axis = p.String(),
                        Addition = p.String(),
                        Index = p.String(),
                        LensNumberCylindricalValue = p.String(),
                        LensNumberSphericalValue = p.String(),
                    },
                body:
                    @"INSERT [dbo].[Lines]([LineUnitPrice], [LineQuantity], [LocalizationID], [ProductID], [isPost], [OeilDroiteGauche], [SupplyingName], [isGift], [isCommandGlass], [NumeroSerie])
                      VALUES (@LineUnitPrice, @LineQuantity, @LocalizationID, @ProductID, @isPost, @OeilDroiteGauche, @SupplyingName, @isGift, @isCommandGlass, @NumeroSerie)
                      
                      DECLARE @LineID int
                      SELECT @LineID = [LineID]
                      FROM [dbo].[Lines]
                      WHERE @@ROWCOUNT > 0 AND [LineID] = scope_identity()
                      
                      INSERT [dbo].[AuthoriseSaleLines]([LineID], [AuthoriseSaleID], [SpecialOrderLineCode], [marque], [reference], [Axis], [Addition], [Index], [LensNumberCylindricalValue], [LensNumberSphericalValue])
                      VALUES (@LineID, @AuthoriseSaleID, @SpecialOrderLineCode, @marque, @reference, @Axis, @Addition, @Index, @LensNumberCylindricalValue, @LensNumberSphericalValue)
                      
                      SELECT t0.[LineID]
                      FROM [dbo].[Lines] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[LineID] = @LineID"
            );
            
            CreateStoredProcedure(
                "dbo.AuthoriseSaleLine_Update",
                p => new
                    {
                        LineID = p.Int(),
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        AuthoriseSaleID = p.Int(),
                        SpecialOrderLineCode = p.String(),
                        marque = p.String(),
                        reference = p.String(),
                        Axis = p.String(),
                        Addition = p.String(),
                        Index = p.String(),
                        LensNumberCylindricalValue = p.String(),
                        LensNumberSphericalValue = p.String(),
                    },
                body:
                    @"UPDATE [dbo].[AuthoriseSaleLines]
                      SET [AuthoriseSaleID] = @AuthoriseSaleID, [SpecialOrderLineCode] = @SpecialOrderLineCode, [marque] = @marque, [reference] = @reference, [Axis] = @Axis, [Addition] = @Addition, [Index] = @Index, [LensNumberCylindricalValue] = @LensNumberCylindricalValue, [LensNumberSphericalValue] = @LensNumberSphericalValue
                      WHERE ([LineID] = @LineID)
                      
                      UPDATE [dbo].[Lines]
                      SET [LineUnitPrice] = @LineUnitPrice, [LineQuantity] = @LineQuantity, [LocalizationID] = @LocalizationID, [ProductID] = @ProductID, [isPost] = @isPost, [OeilDroiteGauche] = @OeilDroiteGauche, [SupplyingName] = @SupplyingName, [isGift] = @isGift, [isCommandGlass] = @isCommandGlass, [NumeroSerie] = @NumeroSerie
                      WHERE ([LineID] = @LineID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.AuthoriseSaleLine_Delete",
                p => new
                    {
                        LineID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[AuthoriseSaleLines]
                      WHERE ([LineID] = @LineID)
                      
                      DELETE [dbo].[Lines]
                      WHERE ([LineID] = @LineID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.CustomerOrderLine_Insert",
                p => new
                    {
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        CustomerOrderID = p.Int(),
                        SpecialOrderLineCode = p.String(),
                        marque = p.String(),
                        reference = p.String(),
                        FrameCategory = p.String(),
                        Addition = p.String(),
                        Axis = p.String(),
                        LensNumberCylindricalValue = p.String(),
                        LensNumberSphericalValue = p.String(),
                    },
                body:
                    @"INSERT [dbo].[Lines]([LineUnitPrice], [LineQuantity], [LocalizationID], [ProductID], [isPost], [OeilDroiteGauche], [SupplyingName], [isGift], [isCommandGlass], [NumeroSerie])
                      VALUES (@LineUnitPrice, @LineQuantity, @LocalizationID, @ProductID, @isPost, @OeilDroiteGauche, @SupplyingName, @isGift, @isCommandGlass, @NumeroSerie)
                      
                      DECLARE @LineID int
                      SELECT @LineID = [LineID]
                      FROM [dbo].[Lines]
                      WHERE @@ROWCOUNT > 0 AND [LineID] = scope_identity()
                      
                      INSERT [dbo].[CustomerOrderLines]([LineID], [CustomerOrderID], [SpecialOrderLineCode], [marque], [reference], [FrameCategory], [Addition], [Axis], [LensNumberCylindricalValue], [LensNumberSphericalValue])
                      VALUES (@LineID, @CustomerOrderID, @SpecialOrderLineCode, @marque, @reference, @FrameCategory, @Addition, @Axis, @LensNumberCylindricalValue, @LensNumberSphericalValue)
                      
                      SELECT t0.[LineID]
                      FROM [dbo].[Lines] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[LineID] = @LineID"
            );
            
            CreateStoredProcedure(
                "dbo.CustomerOrderLine_Update",
                p => new
                    {
                        LineID = p.Int(),
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        CustomerOrderID = p.Int(),
                        SpecialOrderLineCode = p.String(),
                        marque = p.String(),
                        reference = p.String(),
                        FrameCategory = p.String(),
                        Addition = p.String(),
                        Axis = p.String(),
                        LensNumberCylindricalValue = p.String(),
                        LensNumberSphericalValue = p.String(),
                    },
                body:
                    @"UPDATE [dbo].[CustomerOrderLines]
                      SET [CustomerOrderID] = @CustomerOrderID, [SpecialOrderLineCode] = @SpecialOrderLineCode, [marque] = @marque, [reference] = @reference, [FrameCategory] = @FrameCategory, [Addition] = @Addition, [Axis] = @Axis, [LensNumberCylindricalValue] = @LensNumberCylindricalValue, [LensNumberSphericalValue] = @LensNumberSphericalValue
                      WHERE ([LineID] = @LineID)
                      
                      UPDATE [dbo].[Lines]
                      SET [LineUnitPrice] = @LineUnitPrice, [LineQuantity] = @LineQuantity, [LocalizationID] = @LocalizationID, [ProductID] = @ProductID, [isPost] = @isPost, [OeilDroiteGauche] = @OeilDroiteGauche, [SupplyingName] = @SupplyingName, [isGift] = @isGift, [isCommandGlass] = @isCommandGlass, [NumeroSerie] = @NumeroSerie
                      WHERE ([LineID] = @LineID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.CustomerOrderLine_Delete",
                p => new
                    {
                        LineID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[CustomerOrderLines]
                      WHERE ([LineID] = @LineID)
                      
                      DELETE [dbo].[Lines]
                      WHERE ([LineID] = @LineID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.SaleLine_Insert",
                p => new
                    {
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        SaleID = p.Int(),
                        SpecialOrderLineCode = p.String(),
                        marque = p.String(),
                        reference = p.String(),
                        Addition = p.String(),
                        Axis = p.String(),
                        LensNumberCylindricalValue = p.String(),
                        LensNumberSphericalValue = p.String(),
                    },
                body:
                    @"INSERT [dbo].[Lines]([LineUnitPrice], [LineQuantity], [LocalizationID], [ProductID], [isPost], [OeilDroiteGauche], [SupplyingName], [isGift], [isCommandGlass], [NumeroSerie])
                      VALUES (@LineUnitPrice, @LineQuantity, @LocalizationID, @ProductID, @isPost, @OeilDroiteGauche, @SupplyingName, @isGift, @isCommandGlass, @NumeroSerie)
                      
                      DECLARE @LineID int
                      SELECT @LineID = [LineID]
                      FROM [dbo].[Lines]
                      WHERE @@ROWCOUNT > 0 AND [LineID] = scope_identity()
                      
                      INSERT [dbo].[SaleLines]([LineID], [SaleID], [SpecialOrderLineCode], [marque], [reference], [Addition], [Axis], [LensNumberCylindricalValue], [LensNumberSphericalValue])
                      VALUES (@LineID, @SaleID, @SpecialOrderLineCode, @marque, @reference, @Addition, @Axis, @LensNumberCylindricalValue, @LensNumberSphericalValue)
                      
                      SELECT t0.[LineID]
                      FROM [dbo].[Lines] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[LineID] = @LineID"
            );
            
            CreateStoredProcedure(
                "dbo.SaleLine_Update",
                p => new
                    {
                        LineID = p.Int(),
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        SaleID = p.Int(),
                        SpecialOrderLineCode = p.String(),
                        marque = p.String(),
                        reference = p.String(),
                        Addition = p.String(),
                        Axis = p.String(),
                        LensNumberCylindricalValue = p.String(),
                        LensNumberSphericalValue = p.String(),
                    },
                body:
                    @"UPDATE [dbo].[Lines]
                      SET [LineUnitPrice] = @LineUnitPrice, [LineQuantity] = @LineQuantity, [LocalizationID] = @LocalizationID, [ProductID] = @ProductID, [isPost] = @isPost, [OeilDroiteGauche] = @OeilDroiteGauche, [SupplyingName] = @SupplyingName, [isGift] = @isGift, [isCommandGlass] = @isCommandGlass, [NumeroSerie] = @NumeroSerie
                      WHERE ([LineID] = @LineID)
                      
                      UPDATE [dbo].[SaleLines]
                      SET [SaleID] = @SaleID, [SpecialOrderLineCode] = @SpecialOrderLineCode, [marque] = @marque, [reference] = @reference, [Addition] = @Addition, [Axis] = @Axis, [LensNumberCylindricalValue] = @LensNumberCylindricalValue, [LensNumberSphericalValue] = @LensNumberSphericalValue
                      WHERE ([LineID] = @LineID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.SaleLine_Delete",
                p => new
                    {
                        LineID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[SaleLines]
                      WHERE ([LineID] = @LineID)
                      
                      DELETE [dbo].[Lines]
                      WHERE ([LineID] = @LineID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.PurchaseLine_Insert",
                p => new
                    {
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        PurchaseID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Lines]([LineUnitPrice], [LineQuantity], [LocalizationID], [ProductID], [isPost], [OeilDroiteGauche], [SupplyingName], [isGift], [isCommandGlass], [NumeroSerie])
                      VALUES (@LineUnitPrice, @LineQuantity, @LocalizationID, @ProductID, @isPost, @OeilDroiteGauche, @SupplyingName, @isGift, @isCommandGlass, @NumeroSerie)
                      
                      DECLARE @LineID int
                      SELECT @LineID = [LineID]
                      FROM [dbo].[Lines]
                      WHERE @@ROWCOUNT > 0 AND [LineID] = scope_identity()
                      
                      INSERT [dbo].[PurchaseLines]([LineID], [PurchaseID])
                      VALUES (@LineID, @PurchaseID)
                      
                      SELECT t0.[LineID]
                      FROM [dbo].[Lines] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[LineID] = @LineID"
            );
            
            CreateStoredProcedure(
                "dbo.PurchaseLine_Update",
                p => new
                    {
                        LineID = p.Int(),
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        PurchaseID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Lines]
                      SET [LineUnitPrice] = @LineUnitPrice, [LineQuantity] = @LineQuantity, [LocalizationID] = @LocalizationID, [ProductID] = @ProductID, [isPost] = @isPost, [OeilDroiteGauche] = @OeilDroiteGauche, [SupplyingName] = @SupplyingName, [isGift] = @isGift, [isCommandGlass] = @isCommandGlass, [NumeroSerie] = @NumeroSerie
                      WHERE ([LineID] = @LineID)
                      
                      UPDATE [dbo].[PurchaseLines]
                      SET [PurchaseID] = @PurchaseID
                      WHERE ([LineID] = @LineID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.PurchaseLine_Delete",
                p => new
                    {
                        LineID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[PurchaseLines]
                      WHERE ([LineID] = @LineID)
                      
                      DELETE [dbo].[Lines]
                      WHERE ([LineID] = @LineID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.CumulSaleAndBillLine_Insert",
                p => new
                    {
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        CumulSaleAndBillID = p.Int(),
                        SpecialOrderLineCode = p.String(),
                        marque = p.String(),
                        reference = p.String(),
                        Axis = p.String(),
                        Addition = p.String(),
                        Index = p.String(),
                        LensNumberCylindricalValue = p.String(),
                        LensNumberSphericalValue = p.String(),
                        ProductCategoryID = p.Int(),
                        PurchaseLineUnitPrice = p.Double(),
                    },
                body:
                    @"INSERT [dbo].[Lines]([LineUnitPrice], [LineQuantity], [LocalizationID], [ProductID], [isPost], [OeilDroiteGauche], [SupplyingName], [isGift], [isCommandGlass], [NumeroSerie])
                      VALUES (@LineUnitPrice, @LineQuantity, @LocalizationID, @ProductID, @isPost, @OeilDroiteGauche, @SupplyingName, @isGift, @isCommandGlass, @NumeroSerie)
                      
                      DECLARE @LineID int
                      SELECT @LineID = [LineID]
                      FROM [dbo].[Lines]
                      WHERE @@ROWCOUNT > 0 AND [LineID] = scope_identity()
                      
                      INSERT [dbo].[CumulSaleAndBillLines]([LineID], [CumulSaleAndBillID], [SpecialOrderLineCode], [marque], [reference], [Axis], [Addition], [Index], [LensNumberCylindricalValue], [LensNumberSphericalValue], [ProductCategoryID], [PurchaseLineUnitPrice])
                      VALUES (@LineID, @CumulSaleAndBillID, @SpecialOrderLineCode, @marque, @reference, @Axis, @Addition, @Index, @LensNumberCylindricalValue, @LensNumberSphericalValue, @ProductCategoryID, @PurchaseLineUnitPrice)
                      
                      SELECT t0.[LineID]
                      FROM [dbo].[Lines] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[LineID] = @LineID"
            );
            
            CreateStoredProcedure(
                "dbo.CumulSaleAndBillLine_Update",
                p => new
                    {
                        LineID = p.Int(),
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        CumulSaleAndBillID = p.Int(),
                        SpecialOrderLineCode = p.String(),
                        marque = p.String(),
                        reference = p.String(),
                        Axis = p.String(),
                        Addition = p.String(),
                        Index = p.String(),
                        LensNumberCylindricalValue = p.String(),
                        LensNumberSphericalValue = p.String(),
                        ProductCategoryID = p.Int(),
                        PurchaseLineUnitPrice = p.Double(),
                    },
                body:
                    @"UPDATE [dbo].[CumulSaleAndBillLines]
                      SET [CumulSaleAndBillID] = @CumulSaleAndBillID, [SpecialOrderLineCode] = @SpecialOrderLineCode, [marque] = @marque, [reference] = @reference, [Axis] = @Axis, [Addition] = @Addition, [Index] = @Index, [LensNumberCylindricalValue] = @LensNumberCylindricalValue, [LensNumberSphericalValue] = @LensNumberSphericalValue, [ProductCategoryID] = @ProductCategoryID, [PurchaseLineUnitPrice] = @PurchaseLineUnitPrice
                      WHERE ([LineID] = @LineID)
                      
                      UPDATE [dbo].[Lines]
                      SET [LineUnitPrice] = @LineUnitPrice, [LineQuantity] = @LineQuantity, [LocalizationID] = @LocalizationID, [ProductID] = @ProductID, [isPost] = @isPost, [OeilDroiteGauche] = @OeilDroiteGauche, [SupplyingName] = @SupplyingName, [isGift] = @isGift, [isCommandGlass] = @isCommandGlass, [NumeroSerie] = @NumeroSerie
                      WHERE ([LineID] = @LineID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.CumulSaleAndBillLine_Delete",
                p => new
                    {
                        LineID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[CumulSaleAndBillLines]
                      WHERE ([LineID] = @LineID)
                      
                      DELETE [dbo].[Lines]
                      WHERE ([LineID] = @LineID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.Line_Insert",
                p => new
                    {
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                    },
                body:
                    @"INSERT [dbo].[Lines]([LineUnitPrice], [LineQuantity], [LocalizationID], [ProductID], [isPost], [OeilDroiteGauche], [SupplyingName], [isGift], [isCommandGlass], [NumeroSerie])
                      VALUES (@LineUnitPrice, @LineQuantity, @LocalizationID, @ProductID, @isPost, @OeilDroiteGauche, @SupplyingName, @isGift, @isCommandGlass, @NumeroSerie)
                      
                      DECLARE @LineID int
                      SELECT @LineID = [LineID]
                      FROM [dbo].[Lines]
                      WHERE @@ROWCOUNT > 0 AND [LineID] = scope_identity()
                      
                      SELECT t0.[LineID]
                      FROM [dbo].[Lines] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[LineID] = @LineID"
            );
            
            CreateStoredProcedure(
                "dbo.Line_Update",
                p => new
                    {
                        LineID = p.Int(),
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                    },
                body:
                    @"UPDATE [dbo].[Lines]
                      SET [LineUnitPrice] = @LineUnitPrice, [LineQuantity] = @LineQuantity, [LocalizationID] = @LocalizationID, [ProductID] = @ProductID, [isPost] = @isPost, [OeilDroiteGauche] = @OeilDroiteGauche, [SupplyingName] = @SupplyingName, [isGift] = @isGift, [isCommandGlass] = @isCommandGlass, [NumeroSerie] = @NumeroSerie
                      WHERE ([LineID] = @LineID)"
            );
            
            CreateStoredProcedure(
                "dbo.Line_Delete",
                p => new
                    {
                        LineID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Lines]
                      WHERE ([LineID] = @LineID)"
            );
            
            CreateStoredProcedure(
                "dbo.SupplierOrderLine_Insert",
                p => new
                    {
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        SupplierOrderID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Lines]([LineUnitPrice], [LineQuantity], [LocalizationID], [ProductID], [isPost], [OeilDroiteGauche], [SupplyingName], [isGift], [isCommandGlass], [NumeroSerie])
                      VALUES (@LineUnitPrice, @LineQuantity, @LocalizationID, @ProductID, @isPost, @OeilDroiteGauche, @SupplyingName, @isGift, @isCommandGlass, @NumeroSerie)
                      
                      DECLARE @LineID int
                      SELECT @LineID = [LineID]
                      FROM [dbo].[Lines]
                      WHERE @@ROWCOUNT > 0 AND [LineID] = scope_identity()
                      
                      INSERT [dbo].[SupplierOrderLines]([LineID], [SupplierOrderID])
                      VALUES (@LineID, @SupplierOrderID)
                      
                      SELECT t0.[LineID]
                      FROM [dbo].[Lines] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[LineID] = @LineID"
            );
            
            CreateStoredProcedure(
                "dbo.SupplierOrderLine_Update",
                p => new
                    {
                        LineID = p.Int(),
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        SupplierOrderID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Lines]
                      SET [LineUnitPrice] = @LineUnitPrice, [LineQuantity] = @LineQuantity, [LocalizationID] = @LocalizationID, [ProductID] = @ProductID, [isPost] = @isPost, [OeilDroiteGauche] = @OeilDroiteGauche, [SupplyingName] = @SupplyingName, [isGift] = @isGift, [isCommandGlass] = @isCommandGlass, [NumeroSerie] = @NumeroSerie
                      WHERE ([LineID] = @LineID)
                      
                      UPDATE [dbo].[SupplierOrderLines]
                      SET [SupplierOrderID] = @SupplierOrderID
                      WHERE ([LineID] = @LineID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.SupplierOrderLine_Delete",
                p => new
                    {
                        LineID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[SupplierOrderLines]
                      WHERE ([LineID] = @LineID)
                      
                      DELETE [dbo].[Lines]
                      WHERE ([LineID] = @LineID)
                      AND @@ROWCOUNT > 0"
            );
            
            CreateStoredProcedure(
                "dbo.ProductLocalization_Insert",
                p => new
                    {
                        ProductLocalizationStockQuantity = p.Double(),
                        ProductLocalizationSafetyStockQuantity = p.Double(),
                        ProductLocalizationStockSellingPrice = p.Double(),
                        AveragePurchasePrice = p.Double(),
                        ProductLocalizationDate = p.DateTime(),
                        ProductID = p.Int(),
                        LocalizationID = p.Int(),
                        NumeroSerie = p.String(),
                        Marque = p.String(),
                        isDeliver = p.Boolean(),
                        ProductBrand_ProductBrandID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[ProductLocalizations]([ProductLocalizationStockQuantity], [ProductLocalizationSafetyStockQuantity], [ProductLocalizationStockSellingPrice], [AveragePurchasePrice], [ProductLocalizationDate], [ProductID], [LocalizationID], [NumeroSerie], [Marque], [isDeliver], [ProductBrand_ProductBrandID])
                      VALUES (@ProductLocalizationStockQuantity, @ProductLocalizationSafetyStockQuantity, @ProductLocalizationStockSellingPrice, @AveragePurchasePrice, @ProductLocalizationDate, @ProductID, @LocalizationID, @NumeroSerie, @Marque, @isDeliver, @ProductBrand_ProductBrandID)
                      
                      DECLARE @ProductLocalizationID int
                      SELECT @ProductLocalizationID = [ProductLocalizationID]
                      FROM [dbo].[ProductLocalizations]
                      WHERE @@ROWCOUNT > 0 AND [ProductLocalizationID] = scope_identity()
                      
                      SELECT t0.[ProductLocalizationID]
                      FROM [dbo].[ProductLocalizations] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[ProductLocalizationID] = @ProductLocalizationID"
            );
            
            CreateStoredProcedure(
                "dbo.ProductLocalization_Update",
                p => new
                    {
                        ProductLocalizationID = p.Int(),
                        ProductLocalizationStockQuantity = p.Double(),
                        ProductLocalizationSafetyStockQuantity = p.Double(),
                        ProductLocalizationStockSellingPrice = p.Double(),
                        AveragePurchasePrice = p.Double(),
                        ProductLocalizationDate = p.DateTime(),
                        ProductID = p.Int(),
                        LocalizationID = p.Int(),
                        NumeroSerie = p.String(),
                        Marque = p.String(),
                        isDeliver = p.Boolean(),
                        ProductBrand_ProductBrandID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[ProductLocalizations]
                      SET [ProductLocalizationStockQuantity] = @ProductLocalizationStockQuantity, [ProductLocalizationSafetyStockQuantity] = @ProductLocalizationSafetyStockQuantity, [ProductLocalizationStockSellingPrice] = @ProductLocalizationStockSellingPrice, [AveragePurchasePrice] = @AveragePurchasePrice, [ProductLocalizationDate] = @ProductLocalizationDate, [ProductID] = @ProductID, [LocalizationID] = @LocalizationID, [NumeroSerie] = @NumeroSerie, [Marque] = @Marque, [isDeliver] = @isDeliver, [ProductBrand_ProductBrandID] = @ProductBrand_ProductBrandID
                      WHERE ([ProductLocalizationID] = @ProductLocalizationID)"
            );
            
            CreateStoredProcedure(
                "dbo.ProductLocalization_Delete",
                p => new
                    {
                        ProductLocalizationID = p.Int(),
                        ProductBrand_ProductBrandID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[ProductLocalizations]
                      WHERE (([ProductLocalizationID] = @ProductLocalizationID) AND (([ProductBrand_ProductBrandID] = @ProductBrand_ProductBrandID) OR ([ProductBrand_ProductBrandID] IS NULL AND @ProductBrand_ProductBrandID IS NULL)))"
            );
            
            CreateStoredProcedure(
                "dbo.CustomerOrder_Insert",
                p => new
                    {
                        CompteurFacture = p.Int(),
                        CustomerOrderDate = p.DateTime(),
                        CustomerDateHours = p.DateTime(),
                        VatRate = p.Double(),
                        RateReduction = p.Double(),
                        RateDiscount = p.Double(),
                        Patient = p.String(),
                        CustomerOrderNumber = p.String(maxLength: 100),
                        IsDelivered = p.Boolean(),
                        CustomerName = p.String(),
                        PhoneNumber = p.String(),
                        PoliceAssurance = p.String(maxLength: 250),
                        CompanyName = p.String(maxLength: 250),
                        AssureurID = p.Int(),
                        DeviseID = p.Int(),
                        BranchID = p.Int(),
                        OperatorID = p.Int(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        Transport = p.Double(),
                        PlafondAssurance = p.Double(),
                        NumeroBonPriseEnCharge = p.String(),
                        VerreAssurance = p.Double(),
                        MontureAssurance = p.Double(),
                        Plafond = p.Double(),
                        TotalMalade = p.Double(),
                        NumeroFacture = p.String(maxLength: 100),
                        BillState = p.Int(),
                        DatailBill = p.Int(),
                        MntValidate = p.Double(),
                        GestionnaireID = p.Int(),
                        ValidateBillDate = p.DateTime(),
                        BorderoDepotID = p.Int(),
                        DeleteReason = p.String(),
                        DeleteBillDate = p.DateTime(),
                        DeletedByID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[CustomerOrders]([CompteurFacture], [CustomerOrderDate], [CustomerDateHours], [VatRate], [RateReduction], [RateDiscount], [Patient], [CustomerOrderNumber], [IsDelivered], [CustomerName], [PhoneNumber], [PoliceAssurance], [CompanyName], [AssureurID], [DeviseID], [BranchID], [OperatorID], [Remarque], [MedecinTraitant], [Transport], [PlafondAssurance], [NumeroBonPriseEnCharge], [VerreAssurance], [MontureAssurance], [Plafond], [TotalMalade], [NumeroFacture], [BillState], [DatailBill], [MntValidate], [GestionnaireID], [ValidateBillDate], [BorderoDepotID], [DeleteReason], [DeleteBillDate], [DeletedByID])
                      VALUES (@CompteurFacture, @CustomerOrderDate, @CustomerDateHours, @VatRate, @RateReduction, @RateDiscount, @Patient, @CustomerOrderNumber, @IsDelivered, @CustomerName, @PhoneNumber, @PoliceAssurance, @CompanyName, @AssureurID, @DeviseID, @BranchID, @OperatorID, @Remarque, @MedecinTraitant, @Transport, @PlafondAssurance, @NumeroBonPriseEnCharge, @VerreAssurance, @MontureAssurance, @Plafond, @TotalMalade, @NumeroFacture, @BillState, @DatailBill, @MntValidate, @GestionnaireID, @ValidateBillDate, @BorderoDepotID, @DeleteReason, @DeleteBillDate, @DeletedByID)
                      
                      DECLARE @CustomerOrderID int
                      SELECT @CustomerOrderID = [CustomerOrderID]
                      FROM [dbo].[CustomerOrders]
                      WHERE @@ROWCOUNT > 0 AND [CustomerOrderID] = scope_identity()
                      
                      SELECT t0.[CustomerOrderID]
                      FROM [dbo].[CustomerOrders] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[CustomerOrderID] = @CustomerOrderID"
            );
            
            CreateStoredProcedure(
                "dbo.CustomerOrder_Update",
                p => new
                    {
                        CustomerOrderID = p.Int(),
                        CompteurFacture = p.Int(),
                        CustomerOrderDate = p.DateTime(),
                        CustomerDateHours = p.DateTime(),
                        VatRate = p.Double(),
                        RateReduction = p.Double(),
                        RateDiscount = p.Double(),
                        Patient = p.String(),
                        CustomerOrderNumber = p.String(maxLength: 100),
                        IsDelivered = p.Boolean(),
                        CustomerName = p.String(),
                        PhoneNumber = p.String(),
                        PoliceAssurance = p.String(maxLength: 250),
                        CompanyName = p.String(maxLength: 250),
                        AssureurID = p.Int(),
                        DeviseID = p.Int(),
                        BranchID = p.Int(),
                        OperatorID = p.Int(),
                        Remarque = p.String(),
                        MedecinTraitant = p.String(),
                        Transport = p.Double(),
                        PlafondAssurance = p.Double(),
                        NumeroBonPriseEnCharge = p.String(),
                        VerreAssurance = p.Double(),
                        MontureAssurance = p.Double(),
                        Plafond = p.Double(),
                        TotalMalade = p.Double(),
                        NumeroFacture = p.String(maxLength: 100),
                        BillState = p.Int(),
                        DatailBill = p.Int(),
                        MntValidate = p.Double(),
                        GestionnaireID = p.Int(),
                        ValidateBillDate = p.DateTime(),
                        BorderoDepotID = p.Int(),
                        DeleteReason = p.String(),
                        DeleteBillDate = p.DateTime(),
                        DeletedByID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[CustomerOrders]
                      SET [CompteurFacture] = @CompteurFacture, [CustomerOrderDate] = @CustomerOrderDate, [CustomerDateHours] = @CustomerDateHours, [VatRate] = @VatRate, [RateReduction] = @RateReduction, [RateDiscount] = @RateDiscount, [Patient] = @Patient, [CustomerOrderNumber] = @CustomerOrderNumber, [IsDelivered] = @IsDelivered, [CustomerName] = @CustomerName, [PhoneNumber] = @PhoneNumber, [PoliceAssurance] = @PoliceAssurance, [CompanyName] = @CompanyName, [AssureurID] = @AssureurID, [DeviseID] = @DeviseID, [BranchID] = @BranchID, [OperatorID] = @OperatorID, [Remarque] = @Remarque, [MedecinTraitant] = @MedecinTraitant, [Transport] = @Transport, [PlafondAssurance] = @PlafondAssurance, [NumeroBonPriseEnCharge] = @NumeroBonPriseEnCharge, [VerreAssurance] = @VerreAssurance, [MontureAssurance] = @MontureAssurance, [Plafond] = @Plafond, [TotalMalade] = @TotalMalade, [NumeroFacture] = @NumeroFacture, [BillState] = @BillState, [DatailBill] = @DatailBill, [MntValidate] = @MntValidate, [GestionnaireID] = @GestionnaireID, [ValidateBillDate] = @ValidateBillDate, [BorderoDepotID] = @BorderoDepotID, [DeleteReason] = @DeleteReason, [DeleteBillDate] = @DeleteBillDate, [DeletedByID] = @DeletedByID
                      WHERE ([CustomerOrderID] = @CustomerOrderID)"
            );
            
            CreateStoredProcedure(
                "dbo.CustomerOrder_Delete",
                p => new
                    {
                        CustomerOrderID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[CustomerOrders]
                      WHERE ([CustomerOrderID] = @CustomerOrderID)"
            );
            
            CreateStoredProcedure(
                "dbo.Deposit_Insert",
                p => new
                    {
                        Amount = p.Double(),
                        DepositDate = p.DateTime(),
                        PaymentMethodID = p.Int(),
                        DeviseID = p.Int(),
                        SavingAccountID = p.Int(),
                        Representant = p.String(),
                        DepositReference = p.String(maxLength: 100),
                    },
                body:
                    @"INSERT [dbo].[Deposits]([Amount], [DepositDate], [PaymentMethodID], [DeviseID], [SavingAccountID], [Representant], [DepositReference])
                      VALUES (@Amount, @DepositDate, @PaymentMethodID, @DeviseID, @SavingAccountID, @Representant, @DepositReference)
                      
                      DECLARE @DepositID int
                      SELECT @DepositID = [DepositID]
                      FROM [dbo].[Deposits]
                      WHERE @@ROWCOUNT > 0 AND [DepositID] = scope_identity()
                      
                      SELECT t0.[DepositID]
                      FROM [dbo].[Deposits] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[DepositID] = @DepositID"
            );
            
            CreateStoredProcedure(
                "dbo.Deposit_Update",
                p => new
                    {
                        DepositID = p.Int(),
                        Amount = p.Double(),
                        DepositDate = p.DateTime(),
                        PaymentMethodID = p.Int(),
                        DeviseID = p.Int(),
                        SavingAccountID = p.Int(),
                        Representant = p.String(),
                        DepositReference = p.String(maxLength: 100),
                    },
                body:
                    @"UPDATE [dbo].[Deposits]
                      SET [Amount] = @Amount, [DepositDate] = @DepositDate, [PaymentMethodID] = @PaymentMethodID, [DeviseID] = @DeviseID, [SavingAccountID] = @SavingAccountID, [Representant] = @Representant, [DepositReference] = @DepositReference
                      WHERE ([DepositID] = @DepositID)"
            );
            
            CreateStoredProcedure(
                "dbo.Deposit_Delete",
                p => new
                    {
                        DepositID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Deposits]
                      WHERE ([DepositID] = @DepositID)"
            );
            
            CreateStoredProcedure(
                "dbo.AllDeposit_Insert",
                p => new
                    {
                        Amount = p.Double(),
                        AllDepositDate = p.DateTime(),
                        PaymentMethodID = p.Int(),
                        DeviseID = p.Int(),
                        CustomerID = p.Int(),
                        Representant = p.String(),
                        AllDepositReference = p.String(maxLength: 100),
                        BranchID = p.Int(),
                        AllDepositReason = p.String(),
                        IsSpecialOrder = p.Boolean(),
                        CustomerOrderID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[AllDeposits]([Amount], [AllDepositDate], [PaymentMethodID], [DeviseID], [CustomerID], [Representant], [AllDepositReference], [BranchID], [AllDepositReason], [IsSpecialOrder], [CustomerOrderID])
                      VALUES (@Amount, @AllDepositDate, @PaymentMethodID, @DeviseID, @CustomerID, @Representant, @AllDepositReference, @BranchID, @AllDepositReason, @IsSpecialOrder, @CustomerOrderID)
                      
                      DECLARE @AllDepositID int
                      SELECT @AllDepositID = [AllDepositID]
                      FROM [dbo].[AllDeposits]
                      WHERE @@ROWCOUNT > 0 AND [AllDepositID] = scope_identity()
                      
                      SELECT t0.[AllDepositID]
                      FROM [dbo].[AllDeposits] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[AllDepositID] = @AllDepositID"
            );
            
            CreateStoredProcedure(
                "dbo.AllDeposit_Update",
                p => new
                    {
                        AllDepositID = p.Int(),
                        Amount = p.Double(),
                        AllDepositDate = p.DateTime(),
                        PaymentMethodID = p.Int(),
                        DeviseID = p.Int(),
                        CustomerID = p.Int(),
                        Representant = p.String(),
                        AllDepositReference = p.String(maxLength: 100),
                        BranchID = p.Int(),
                        AllDepositReason = p.String(),
                        IsSpecialOrder = p.Boolean(),
                        CustomerOrderID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[AllDeposits]
                      SET [Amount] = @Amount, [AllDepositDate] = @AllDepositDate, [PaymentMethodID] = @PaymentMethodID, [DeviseID] = @DeviseID, [CustomerID] = @CustomerID, [Representant] = @Representant, [AllDepositReference] = @AllDepositReference, [BranchID] = @BranchID, [AllDepositReason] = @AllDepositReason, [IsSpecialOrder] = @IsSpecialOrder, [CustomerOrderID] = @CustomerOrderID
                      WHERE ([AllDepositID] = @AllDepositID)"
            );
            
            CreateStoredProcedure(
                "dbo.AllDeposit_Delete",
                p => new
                    {
                        AllDepositID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[AllDeposits]
                      WHERE ([AllDepositID] = @AllDepositID)"
            );
            
            CreateStoredProcedure(
                "dbo.InventoryHistoric_Insert",
                p => new
                    {
                        InventoryDate = p.DateTime(),
                        inventoryReason = p.String(),
                        OldStockQuantity = p.Double(),
                        NewStockQuantity = p.Double(),
                        OldStockUnitPrice = p.Double(),
                        NEwStockUnitPrice = p.Double(),
                        OldSafetyStockQuantity = p.Double(),
                        NewSafetyStockQuantity = p.Double(),
                        ProductID = p.Int(),
                        LocalizationID = p.Int(),
                        AutorizedByID = p.Int(),
                        CountByID = p.Int(),
                        RegisteredByID = p.Int(),
                        StockStatus = p.String(maxLength: 6),
                        Description = p.String(),
                        Quantity = p.Double(),
                        NumeroSerie = p.String(),
                        Marque = p.String(),
                    },
                body:
                    @"INSERT [dbo].[InventoryHistorics]([InventoryDate], [inventoryReason], [OldStockQuantity], [NewStockQuantity], [OldStockUnitPrice], [NEwStockUnitPrice], [OldSafetyStockQuantity], [NewSafetyStockQuantity], [ProductID], [LocalizationID], [AutorizedByID], [CountByID], [RegisteredByID], [StockStatus], [Description], [Quantity], [NumeroSerie], [Marque])
                      VALUES (@InventoryDate, @inventoryReason, @OldStockQuantity, @NewStockQuantity, @OldStockUnitPrice, @NEwStockUnitPrice, @OldSafetyStockQuantity, @NewSafetyStockQuantity, @ProductID, @LocalizationID, @AutorizedByID, @CountByID, @RegisteredByID, @StockStatus, @Description, @Quantity, @NumeroSerie, @Marque)
                      
                      DECLARE @InventoryHistoricID int
                      SELECT @InventoryHistoricID = [InventoryHistoricID]
                      FROM [dbo].[InventoryHistorics]
                      WHERE @@ROWCOUNT > 0 AND [InventoryHistoricID] = scope_identity()
                      
                      SELECT t0.[InventoryHistoricID]
                      FROM [dbo].[InventoryHistorics] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[InventoryHistoricID] = @InventoryHistoricID"
            );
            
            CreateStoredProcedure(
                "dbo.InventoryHistoric_Update",
                p => new
                    {
                        InventoryHistoricID = p.Int(),
                        InventoryDate = p.DateTime(),
                        inventoryReason = p.String(),
                        OldStockQuantity = p.Double(),
                        NewStockQuantity = p.Double(),
                        OldStockUnitPrice = p.Double(),
                        NEwStockUnitPrice = p.Double(),
                        OldSafetyStockQuantity = p.Double(),
                        NewSafetyStockQuantity = p.Double(),
                        ProductID = p.Int(),
                        LocalizationID = p.Int(),
                        AutorizedByID = p.Int(),
                        CountByID = p.Int(),
                        RegisteredByID = p.Int(),
                        StockStatus = p.String(maxLength: 6),
                        Description = p.String(),
                        Quantity = p.Double(),
                        NumeroSerie = p.String(),
                        Marque = p.String(),
                    },
                body:
                    @"UPDATE [dbo].[InventoryHistorics]
                      SET [InventoryDate] = @InventoryDate, [inventoryReason] = @inventoryReason, [OldStockQuantity] = @OldStockQuantity, [NewStockQuantity] = @NewStockQuantity, [OldStockUnitPrice] = @OldStockUnitPrice, [NEwStockUnitPrice] = @NEwStockUnitPrice, [OldSafetyStockQuantity] = @OldSafetyStockQuantity, [NewSafetyStockQuantity] = @NewSafetyStockQuantity, [ProductID] = @ProductID, [LocalizationID] = @LocalizationID, [AutorizedByID] = @AutorizedByID, [CountByID] = @CountByID, [RegisteredByID] = @RegisteredByID, [StockStatus] = @StockStatus, [Description] = @Description, [Quantity] = @Quantity, [NumeroSerie] = @NumeroSerie, [Marque] = @Marque
                      WHERE ([InventoryHistoricID] = @InventoryHistoricID)"
            );
            
            CreateStoredProcedure(
                "dbo.InventoryHistoric_Delete",
                p => new
                    {
                        InventoryHistoricID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[InventoryHistorics]
                      WHERE ([InventoryHistoricID] = @InventoryHistoricID)"
            );
            
        }
        
        public override void Down()
        {
            DropStoredProcedure("dbo.InventoryHistoric_Delete");
            DropStoredProcedure("dbo.InventoryHistoric_Update");
            DropStoredProcedure("dbo.InventoryHistoric_Insert");
            DropStoredProcedure("dbo.AllDeposit_Delete");
            DropStoredProcedure("dbo.AllDeposit_Update");
            DropStoredProcedure("dbo.AllDeposit_Insert");
            DropStoredProcedure("dbo.Deposit_Delete");
            DropStoredProcedure("dbo.Deposit_Update");
            DropStoredProcedure("dbo.Deposit_Insert");
            DropStoredProcedure("dbo.CustomerOrder_Delete");
            DropStoredProcedure("dbo.CustomerOrder_Update");
            DropStoredProcedure("dbo.CustomerOrder_Insert");
            DropStoredProcedure("dbo.ProductLocalization_Delete");
            DropStoredProcedure("dbo.ProductLocalization_Update");
            DropStoredProcedure("dbo.ProductLocalization_Insert");
            DropStoredProcedure("dbo.SupplierOrderLine_Delete");
            DropStoredProcedure("dbo.SupplierOrderLine_Update");
            DropStoredProcedure("dbo.SupplierOrderLine_Insert");
            DropStoredProcedure("dbo.Line_Delete");
            DropStoredProcedure("dbo.Line_Update");
            DropStoredProcedure("dbo.Line_Insert");
            DropStoredProcedure("dbo.CumulSaleAndBillLine_Delete");
            DropStoredProcedure("dbo.CumulSaleAndBillLine_Update");
            DropStoredProcedure("dbo.CumulSaleAndBillLine_Insert");
            DropStoredProcedure("dbo.PurchaseLine_Delete");
            DropStoredProcedure("dbo.PurchaseLine_Update");
            DropStoredProcedure("dbo.PurchaseLine_Insert");
            DropStoredProcedure("dbo.SaleLine_Delete");
            DropStoredProcedure("dbo.SaleLine_Update");
            DropStoredProcedure("dbo.SaleLine_Insert");
            DropStoredProcedure("dbo.CustomerOrderLine_Delete");
            DropStoredProcedure("dbo.CustomerOrderLine_Update");
            DropStoredProcedure("dbo.CustomerOrderLine_Insert");
            DropStoredProcedure("dbo.AuthoriseSaleLine_Delete");
            DropStoredProcedure("dbo.AuthoriseSaleLine_Update");
            DropStoredProcedure("dbo.AuthoriseSaleLine_Insert");
            DropStoredProcedure("dbo.SavingAccountSale_Delete");
            DropStoredProcedure("dbo.SavingAccountSale_Update");
            DropStoredProcedure("dbo.SavingAccountSale_Insert");
            DropStoredProcedure("dbo.AssureurSale_Delete");
            DropStoredProcedure("dbo.AssureurSale_Update");
            DropStoredProcedure("dbo.AssureurSale_Insert");
            DropStoredProcedure("dbo.TillSale_Delete");
            DropStoredProcedure("dbo.TillSale_Update");
            DropStoredProcedure("dbo.TillSale_Insert");
            DropStoredProcedure("dbo.BankSale_Delete");
            DropStoredProcedure("dbo.BankSale_Update");
            DropStoredProcedure("dbo.BankSale_Insert");
            DropStoredProcedure("dbo.Sale_Delete");
            DropStoredProcedure("dbo.Sale_Update");
            DropStoredProcedure("dbo.Sale_Insert");
            DropStoredProcedure("dbo.Supplier_Delete");
            DropStoredProcedure("dbo.Supplier_Update");
            DropStoredProcedure("dbo.Supplier_Insert");
            DropStoredProcedure("dbo.Customer_Delete");
            DropStoredProcedure("dbo.Customer_Update");
            DropStoredProcedure("dbo.Customer_Insert");
            DropStoredProcedure("dbo.Assureur_Delete");
            DropStoredProcedure("dbo.Assureur_Update");
            DropStoredProcedure("dbo.Assureur_Insert");
            DropStoredProcedure("dbo.User_Delete");
            DropStoredProcedure("dbo.User_Update");
            DropStoredProcedure("dbo.User_Insert");
            DropStoredProcedure("dbo.Company_Delete");
            DropStoredProcedure("dbo.Company_Update");
            DropStoredProcedure("dbo.Company_Insert");
            DropStoredProcedure("dbo.TreasuryOperationAccountOperation_Delete");
            DropStoredProcedure("dbo.TreasuryOperationAccountOperation_Update");
            DropStoredProcedure("dbo.TreasuryOperationAccountOperation_Insert");
            DropStoredProcedure("dbo.TillAdjustAccountOperation_Delete");
            DropStoredProcedure("dbo.TillAdjustAccountOperation_Update");
            DropStoredProcedure("dbo.TillAdjustAccountOperation_Insert");
            DropStoredProcedure("dbo.PurchaseReturnAccountOperation_Delete");
            DropStoredProcedure("dbo.PurchaseReturnAccountOperation_Update");
            DropStoredProcedure("dbo.PurchaseReturnAccountOperation_Insert");
            DropStoredProcedure("dbo.PurchaseAccountOperation_Delete");
            DropStoredProcedure("dbo.PurchaseAccountOperation_Update");
            DropStoredProcedure("dbo.PurchaseAccountOperation_Insert");
            DropStoredProcedure("dbo.ProductTransferAccountOperation_Delete");
            DropStoredProcedure("dbo.ProductTransferAccountOperation_Update");
            DropStoredProcedure("dbo.ProductTransferAccountOperation_Insert");
            DropStoredProcedure("dbo.ProductLocalizationAccountOperation_Delete");
            DropStoredProcedure("dbo.ProductLocalizationAccountOperation_Update");
            DropStoredProcedure("dbo.ProductLocalizationAccountOperation_Insert");
            DropStoredProcedure("dbo.BudgetConsumptionAccountOperation_Delete");
            DropStoredProcedure("dbo.BudgetConsumptionAccountOperation_Update");
            DropStoredProcedure("dbo.BudgetConsumptionAccountOperation_Insert");
            DropStoredProcedure("dbo.SaleAccountOperation_Delete");
            DropStoredProcedure("dbo.SaleAccountOperation_Update");
            DropStoredProcedure("dbo.SaleAccountOperation_Insert");
            DropStoredProcedure("dbo.SaleReturnAccountOperation_Delete");
            DropStoredProcedure("dbo.SaleReturnAccountOperation_Update");
            DropStoredProcedure("dbo.SaleReturnAccountOperation_Insert");
            DropStoredProcedure("dbo.ManualAccountOperation_Delete");
            DropStoredProcedure("dbo.ManualAccountOperation_Update");
            DropStoredProcedure("dbo.ManualAccountOperation_Insert");
            DropStoredProcedure("dbo.DepositAccountOperation_Delete");
            DropStoredProcedure("dbo.DepositAccountOperation_Update");
            DropStoredProcedure("dbo.DepositAccountOperation_Insert");
            DropStoredProcedure("dbo.AccountOperation_Delete");
            DropStoredProcedure("dbo.AccountOperation_Update");
            DropStoredProcedure("dbo.AccountOperation_Insert");
        }
    }
}
