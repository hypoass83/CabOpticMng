namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AllDeposit_CustomerOrderID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AllDeposits", "CustomerOrderID", c => c.Int());
            CreateIndex("dbo.AllDeposits", "CustomerOrderID");
            AddForeignKey("dbo.AllDeposits", "CustomerOrderID", "dbo.CustomerOrders", "CustomerOrderID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AllDeposits", "CustomerOrderID", "dbo.CustomerOrders");
            DropIndex("dbo.AllDeposits", new[] { "CustomerOrderID" });
            DropColumn("dbo.AllDeposits", "CustomerOrderID");
        }
    }
}
