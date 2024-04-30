namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TreasuryOperation_Tables : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AccountOperations", new[] { "TillAdjustID" });
            CreateTable(
                "dbo.TreasuryOperations",
                c => new
                    {
                        TreasuryOperationID = c.Int(nullable: false, identity: true),
                        BranchID = c.Int(nullable: false),
                        OperationDate = c.DateTime(nullable: false),
                        ComputerPrice = c.Double(nullable: false),
                        OperationRef = c.String(),
                        OperationType = c.String(),
                        Justification = c.String(),
                        OperationAmount = c.Double(nullable: false),
                        TillID = c.Int(nullable: false),
                        TillDestID = c.Int(),
                        DeviseID = c.Int(nullable: false),
                        BankID = c.Int(),
                    })
                .PrimaryKey(t => t.TreasuryOperationID)
                .ForeignKey("dbo.Banks", t => t.BankID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .ForeignKey("dbo.Devises", t => t.DeviseID)
                .ForeignKey("dbo.Tills", t => t.TillID)
                .ForeignKey("dbo.Tills", t => t.TillDestID)
                .Index(t => t.BranchID)
                .Index(t => t.TillID)
                .Index(t => t.TillDestID)
                .Index(t => t.DeviseID)
                .Index(t => t.BankID);
            
            CreateTable(
                "dbo.TillAdjustAccountOperations",
                c => new
                    {
                        AccountOperationID = c.Long(nullable: false),
                        TillAdjustID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountOperationID)
                .ForeignKey("dbo.AccountOperations", t => t.AccountOperationID)
                .Index(t => t.AccountOperationID)
                .Index(t => t.TillAdjustID);
            
            CreateTable(
                "dbo.TreasuryOperationAccountOperations",
                c => new
                    {
                        AccountOperationID = c.Long(nullable: false),
                        TreasuryOperationID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountOperationID)
                .ForeignKey("dbo.AccountOperations", t => t.AccountOperationID)
                .ForeignKey("dbo.TreasuryOperations", t => t.TreasuryOperationID)
                .Index(t => t.AccountOperationID)
                .Index(t => t.TreasuryOperationID);
            
            DropColumn("dbo.AccountOperations", "TillAdjustID");
            DropColumn("dbo.AccountOperations", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AccountOperations", "Discriminator", c => c.String(maxLength: 128));
            AddColumn("dbo.AccountOperations", "TillAdjustID", c => c.Int());
            DropForeignKey("dbo.TreasuryOperationAccountOperations", "TreasuryOperationID", "dbo.TreasuryOperations");
            DropForeignKey("dbo.TreasuryOperationAccountOperations", "AccountOperationID", "dbo.AccountOperations");
            DropForeignKey("dbo.TillAdjustAccountOperations", "AccountOperationID", "dbo.AccountOperations");
            DropForeignKey("dbo.TreasuryOperations", "TillDestID", "dbo.Tills");
            DropForeignKey("dbo.TreasuryOperations", "TillID", "dbo.Tills");
            DropForeignKey("dbo.TreasuryOperations", "DeviseID", "dbo.Devises");
            DropForeignKey("dbo.TreasuryOperations", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.TreasuryOperations", "BankID", "dbo.Banks");
            DropIndex("dbo.TreasuryOperationAccountOperations", new[] { "TreasuryOperationID" });
            DropIndex("dbo.TreasuryOperationAccountOperations", new[] { "AccountOperationID" });
            DropIndex("dbo.TillAdjustAccountOperations", new[] { "TillAdjustID" });
            DropIndex("dbo.TillAdjustAccountOperations", new[] { "AccountOperationID" });
            DropIndex("dbo.TreasuryOperations", new[] { "BankID" });
            DropIndex("dbo.TreasuryOperations", new[] { "DeviseID" });
            DropIndex("dbo.TreasuryOperations", new[] { "TillDestID" });
            DropIndex("dbo.TreasuryOperations", new[] { "TillID" });
            DropIndex("dbo.TreasuryOperations", new[] { "BranchID" });
            DropTable("dbo.TreasuryOperationAccountOperations");
            DropTable("dbo.TillAdjustAccountOperations");
            DropTable("dbo.TreasuryOperations");
            CreateIndex("dbo.AccountOperations", "TillAdjustID");
        }
    }
}
