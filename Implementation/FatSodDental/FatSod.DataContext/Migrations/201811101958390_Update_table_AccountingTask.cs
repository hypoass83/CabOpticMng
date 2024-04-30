namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_table_AccountingTask : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.AccountingTask", newName: "AccountingTasks");
            DropForeignKey("dbo.AccountingSpecificTask", "AccountingTaskID", "dbo.AccountingTask");
            DropIndex("dbo.AccountingSpecificTask", new[] { "AccountingTaskID" });
            DropIndex("dbo.AccountingSpecificTask", new[] { "AccountID" });
            DropIndex("dbo.AccountingSpecificTask", new[] { "VatAccountID" });
            DropIndex("dbo.AccountingSpecificTask", new[] { "DiscountAccountID" });
            DropIndex("dbo.AccountingSpecificTask", new[] { "TransportAccountID" });
            AddColumn("dbo.AccountingTasks", "AccountID", c => c.Int());
            AddColumn("dbo.AccountingTasks", "VatAccountID", c => c.Int());
            AddColumn("dbo.AccountingTasks", "DiscountAccountID", c => c.Int());
            AddColumn("dbo.AccountingTasks", "TransportAccountID", c => c.Int());
            CreateIndex("dbo.AccountingTasks", "AccountID");
            CreateIndex("dbo.AccountingTasks", "VatAccountID");
            CreateIndex("dbo.AccountingTasks", "DiscountAccountID");
            CreateIndex("dbo.AccountingTasks", "TransportAccountID");
            DropTable("dbo.AccountingSpecificTask");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.AccountingSpecificTask",
                c => new
                    {
                        AccountingTaskID = c.Int(nullable: false),
                        AccountID = c.Int(),
                        VatAccountID = c.Int(),
                        DiscountAccountID = c.Int(),
                        TransportAccountID = c.Int(),
                    })
                .PrimaryKey(t => t.AccountingTaskID);
            
            DropIndex("dbo.AccountingTasks", new[] { "TransportAccountID" });
            DropIndex("dbo.AccountingTasks", new[] { "DiscountAccountID" });
            DropIndex("dbo.AccountingTasks", new[] { "VatAccountID" });
            DropIndex("dbo.AccountingTasks", new[] { "AccountID" });
            DropColumn("dbo.AccountingTasks", "TransportAccountID");
            DropColumn("dbo.AccountingTasks", "DiscountAccountID");
            DropColumn("dbo.AccountingTasks", "VatAccountID");
            DropColumn("dbo.AccountingTasks", "AccountID");
            CreateIndex("dbo.AccountingSpecificTask", "TransportAccountID");
            CreateIndex("dbo.AccountingSpecificTask", "DiscountAccountID");
            CreateIndex("dbo.AccountingSpecificTask", "VatAccountID");
            CreateIndex("dbo.AccountingSpecificTask", "AccountID");
            CreateIndex("dbo.AccountingSpecificTask", "AccountingTaskID");
            AddForeignKey("dbo.AccountingSpecificTask", "AccountingTaskID", "dbo.AccountingTask", "AccountingTaskID");
            RenameTable(name: "dbo.AccountingTasks", newName: "AccountingTask");
        }
    }
}
