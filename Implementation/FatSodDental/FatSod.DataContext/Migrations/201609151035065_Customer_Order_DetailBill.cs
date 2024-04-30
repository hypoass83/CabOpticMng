namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer_Order_DetailBill : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrders", "DatailBill", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerOrders", "DatailBill");
        }
    }
}
