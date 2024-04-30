namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer_Order_Statu_bill : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.CustomerOrders", new[] { "AssureurID" });
            AlterColumn("dbo.CustomerOrders", "AssureurID", c => c.Int());
            AlterColumn("dbo.CustomerOrders", "BillState", c => c.Int(nullable: false));
            CreateIndex("dbo.CustomerOrders", "AssureurID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.CustomerOrders", new[] { "AssureurID" });
            AlterColumn("dbo.CustomerOrders", "BillState", c => c.String());
            AlterColumn("dbo.CustomerOrders", "AssureurID", c => c.Int(nullable: false));
            CreateIndex("dbo.CustomerOrders", "AssureurID");
        }
    }
}
