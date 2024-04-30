namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOperatorIdToFinancialActions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Deposits", "OperatorID", c => c.Int());
            AddColumn("dbo.Slices", "OperatorID", c => c.Int());
            AddColumn("dbo.AllDeposits", "OperatorID", c => c.Int());
            CreateIndex("dbo.Deposits", "OperatorID");
            CreateIndex("dbo.Slices", "OperatorID");
            CreateIndex("dbo.AllDeposits", "OperatorID");
            AddForeignKey("dbo.Deposits", "OperatorID", "dbo.Users", "GlobalPersonID");
            AddForeignKey("dbo.Slices", "OperatorID", "dbo.Users", "GlobalPersonID");
            AddForeignKey("dbo.AllDeposits", "OperatorID", "dbo.Users", "GlobalPersonID");
            AlterStoredProcedure(
                "dbo.Deposit_Insert",
                p => new
                    {
                        Amount = p.Double(),
                        DepositDate = p.DateTime(),
                        PaymentMethodID = p.Int(),
                        DeviseID = p.Int(),
                        SavingAccountID = p.Int(),
                        Representant = p.String(),
                        OperatorID = p.Int(),
                        DepositReference = p.String(maxLength: 100),
                    },
                body:
                    @"INSERT [dbo].[Deposits]([Amount], [DepositDate], [PaymentMethodID], [DeviseID], [SavingAccountID], [Representant], [OperatorID], [DepositReference])
                      VALUES (@Amount, @DepositDate, @PaymentMethodID, @DeviseID, @SavingAccountID, @Representant, @OperatorID, @DepositReference)
                      
                      DECLARE @DepositID int
                      SELECT @DepositID = [DepositID]
                      FROM [dbo].[Deposits]
                      WHERE @@ROWCOUNT > 0 AND [DepositID] = scope_identity()
                      
                      SELECT t0.[DepositID]
                      FROM [dbo].[Deposits] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[DepositID] = @DepositID"
            );
            
            AlterStoredProcedure(
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
                        OperatorID = p.Int(),
                        DepositReference = p.String(maxLength: 100),
                    },
                body:
                    @"UPDATE [dbo].[Deposits]
                      SET [Amount] = @Amount, [DepositDate] = @DepositDate, [PaymentMethodID] = @PaymentMethodID, [DeviseID] = @DeviseID, [SavingAccountID] = @SavingAccountID, [Representant] = @Representant, [OperatorID] = @OperatorID, [DepositReference] = @DepositReference
                      WHERE ([DepositID] = @DepositID)"
            );
            
            AlterStoredProcedure(
                "dbo.AllDeposit_Insert",
                p => new
                    {
                        Amount = p.Double(),
                        AllDepositDate = p.DateTime(),
                        OperatorID = p.Int(),
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
                    @"INSERT [dbo].[AllDeposits]([Amount], [AllDepositDate], [OperatorID], [PaymentMethodID], [DeviseID], [CustomerID], [Representant], [AllDepositReference], [BranchID], [AllDepositReason], [IsSpecialOrder], [CustomerOrderID])
                      VALUES (@Amount, @AllDepositDate, @OperatorID, @PaymentMethodID, @DeviseID, @CustomerID, @Representant, @AllDepositReference, @BranchID, @AllDepositReason, @IsSpecialOrder, @CustomerOrderID)
                      
                      DECLARE @AllDepositID int
                      SELECT @AllDepositID = [AllDepositID]
                      FROM [dbo].[AllDeposits]
                      WHERE @@ROWCOUNT > 0 AND [AllDepositID] = scope_identity()
                      
                      SELECT t0.[AllDepositID]
                      FROM [dbo].[AllDeposits] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[AllDepositID] = @AllDepositID"
            );
            
            AlterStoredProcedure(
                "dbo.AllDeposit_Update",
                p => new
                    {
                        AllDepositID = p.Int(),
                        Amount = p.Double(),
                        AllDepositDate = p.DateTime(),
                        OperatorID = p.Int(),
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
                      SET [Amount] = @Amount, [AllDepositDate] = @AllDepositDate, [OperatorID] = @OperatorID, [PaymentMethodID] = @PaymentMethodID, [DeviseID] = @DeviseID, [CustomerID] = @CustomerID, [Representant] = @Representant, [AllDepositReference] = @AllDepositReference, [BranchID] = @BranchID, [AllDepositReason] = @AllDepositReason, [IsSpecialOrder] = @IsSpecialOrder, [CustomerOrderID] = @CustomerOrderID
                      WHERE ([AllDepositID] = @AllDepositID)"
            );
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AllDeposits", "OperatorID", "dbo.Users");
            DropForeignKey("dbo.Slices", "OperatorID", "dbo.Users");
            DropForeignKey("dbo.Deposits", "OperatorID", "dbo.Users");
            DropIndex("dbo.AllDeposits", new[] { "OperatorID" });
            DropIndex("dbo.Slices", new[] { "OperatorID" });
            DropIndex("dbo.Deposits", new[] { "OperatorID" });
            DropColumn("dbo.AllDeposits", "OperatorID");
            DropColumn("dbo.Slices", "OperatorID");
            DropColumn("dbo.Deposits", "OperatorID");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
