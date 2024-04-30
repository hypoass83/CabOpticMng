namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tab_DepositAccountOperations_add_ref_deposit_Table : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.SavingAccountOperations", "AccountOperationID", "dbo.AccountOperations");
            //DropForeignKey("dbo.SavingAccountOperations", "SavingAccountID", "dbo.SavingAccounts");
            //DropIndex("dbo.SavingAccountOperations", new[] { "AccountOperationID" });
            //DropIndex("dbo.SavingAccountOperations", new[] { "SavingAccountID" });
            CreateTable(
                "dbo.DepositAccountOperations",
                c => new
                    {
                        AccountOperationID = c.Long(nullable: false),
                        DepositID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountOperationID)
                .ForeignKey("dbo.AccountOperations", t => t.AccountOperationID)
                .ForeignKey("dbo.Deposits", t => t.DepositID)
                .Index(t => t.AccountOperationID)
                .Index(t => t.DepositID);
            
            AddColumn("dbo.Deposits", "DepositReference", c => c.String());
            //DropTable("dbo.SavingAccountOperations");
        }
        
        public override void Down()
        {
            //CreateTable(
            //    "dbo.SavingAccountOperations",
            //    c => new
            //        {
            //            AccountOperationID = c.Long(nullable: false),
            //            SavingAccountID = c.Int(nullable: false),
            //        })
            //    .PrimaryKey(t => t.AccountOperationID);
            
            DropForeignKey("dbo.DepositAccountOperations", "DepositID", "dbo.Deposits");
            DropForeignKey("dbo.DepositAccountOperations", "AccountOperationID", "dbo.AccountOperations");
            DropIndex("dbo.DepositAccountOperations", new[] { "DepositID" });
            DropIndex("dbo.DepositAccountOperations", new[] { "AccountOperationID" });
            DropColumn("dbo.Deposits", "DepositReference");
            DropTable("dbo.DepositAccountOperations");
            //CreateIndex("dbo.SavingAccountOperations", "SavingAccountID");
            //CreateIndex("dbo.SavingAccountOperations", "AccountOperationID");
            //AddForeignKey("dbo.SavingAccountOperations", "SavingAccountID", "dbo.SavingAccounts", "ID");
            //AddForeignKey("dbo.SavingAccountOperations", "AccountOperationID", "dbo.AccountOperations", "AccountOperationID");
        }
    }
}
