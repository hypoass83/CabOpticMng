namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DepositDigitalPayment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Deposits", "DigitalAccountManagerId", c => c.Int());
            AddColumn("dbo.Deposits", "TransactionIdentifier", c => c.String());
            AddColumn("dbo.AllDeposits", "DigitalAccountManagerId", c => c.Int());
            AddColumn("dbo.AllDeposits", "TransactionIdentifier", c => c.String());
            CreateIndex("dbo.Deposits", "DigitalAccountManagerId");
            CreateIndex("dbo.AllDeposits", "DigitalAccountManagerId");
            AddForeignKey("dbo.Deposits", "DigitalAccountManagerId", "dbo.Users", "GlobalPersonID");
            AddForeignKey("dbo.AllDeposits", "DigitalAccountManagerId", "dbo.Users", "GlobalPersonID");
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
                        DigitalAccountManagerId = p.Int(),
                        TransactionIdentifier = p.String(),
                        DepositReference = p.String(maxLength: 100),
                    },
                body:
                    @"INSERT [dbo].[Deposits]([Amount], [DepositDate], [PaymentMethodID], [DeviseID], [SavingAccountID], [Representant], [OperatorID], [DigitalAccountManagerId], [TransactionIdentifier], [DepositReference])
                      VALUES (@Amount, @DepositDate, @PaymentMethodID, @DeviseID, @SavingAccountID, @Representant, @OperatorID, @DigitalAccountManagerId, @TransactionIdentifier, @DepositReference)
                      
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
                        DigitalAccountManagerId = p.Int(),
                        TransactionIdentifier = p.String(),
                        DepositReference = p.String(maxLength: 100),
                    },
                body:
                    @"UPDATE [dbo].[Deposits]
                      SET [Amount] = @Amount, [DepositDate] = @DepositDate, [PaymentMethodID] = @PaymentMethodID, [DeviseID] = @DeviseID, [SavingAccountID] = @SavingAccountID, [Representant] = @Representant, [OperatorID] = @OperatorID, [DigitalAccountManagerId] = @DigitalAccountManagerId, [TransactionIdentifier] = @TransactionIdentifier, [DepositReference] = @DepositReference
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
                        DigitalAccountManagerId = p.Int(),
                        TransactionIdentifier = p.String(),
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
                    @"INSERT [dbo].[AllDeposits]([Amount], [AllDepositDate], [OperatorID], [PaymentMethodID], [DigitalAccountManagerId], [TransactionIdentifier], [DeviseID], [CustomerID], [Representant], [AllDepositReference], [BranchID], [AllDepositReason], [IsSpecialOrder], [CustomerOrderID])
                      VALUES (@Amount, @AllDepositDate, @OperatorID, @PaymentMethodID, @DigitalAccountManagerId, @TransactionIdentifier, @DeviseID, @CustomerID, @Representant, @AllDepositReference, @BranchID, @AllDepositReason, @IsSpecialOrder, @CustomerOrderID)
                      
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
                        DigitalAccountManagerId = p.Int(),
                        TransactionIdentifier = p.String(),
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
                      SET [Amount] = @Amount, [AllDepositDate] = @AllDepositDate, [OperatorID] = @OperatorID, [PaymentMethodID] = @PaymentMethodID, [DigitalAccountManagerId] = @DigitalAccountManagerId, [TransactionIdentifier] = @TransactionIdentifier, [DeviseID] = @DeviseID, [CustomerID] = @CustomerID, [Representant] = @Representant, [AllDepositReference] = @AllDepositReference, [BranchID] = @BranchID, [AllDepositReason] = @AllDepositReason, [IsSpecialOrder] = @IsSpecialOrder, [CustomerOrderID] = @CustomerOrderID
                      WHERE ([AllDepositID] = @AllDepositID)"
            );
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AllDeposits", "DigitalAccountManagerId", "dbo.Users");
            DropForeignKey("dbo.Deposits", "DigitalAccountManagerId", "dbo.Users");
            DropIndex("dbo.AllDeposits", new[] { "DigitalAccountManagerId" });
            DropIndex("dbo.Deposits", new[] { "DigitalAccountManagerId" });
            DropColumn("dbo.AllDeposits", "TransactionIdentifier");
            DropColumn("dbo.AllDeposits", "DigitalAccountManagerId");
            DropColumn("dbo.Deposits", "TransactionIdentifier");
            DropColumn("dbo.Deposits", "DigitalAccountManagerId");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
