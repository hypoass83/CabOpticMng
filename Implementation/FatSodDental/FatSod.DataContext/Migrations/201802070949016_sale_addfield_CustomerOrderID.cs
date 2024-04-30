namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sale_addfield_CustomerOrderID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "CustomerOrderID", c => c.Int());
            CreateIndex("dbo.Sales", "CustomerOrderID");
            AddForeignKey("dbo.Sales", "CustomerOrderID", "dbo.CustomerOrders", "CustomerOrderID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sales", "CustomerOrderID", "dbo.CustomerOrders");
            DropIndex("dbo.Sales", new[] { "CustomerOrderID" });
            DropColumn("dbo.Sales", "CustomerOrderID");
        }
    }
}
