namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customerorder_DeleteReason : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrders", "DeleteReason", c => c.String());
            AddColumn("dbo.CustomerOrders", "DeleteBillDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.CustomerOrders", "DeletedByID", c => c.Int());
            CreateIndex("dbo.CustomerOrders", "DeletedByID");
            AddForeignKey("dbo.CustomerOrders", "DeletedByID", "dbo.Users", "GlobalPersonID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerOrders", "DeletedByID", "dbo.Users");
            DropIndex("dbo.CustomerOrders", new[] { "DeletedByID" });
            DropColumn("dbo.CustomerOrders", "DeletedByID");
            DropColumn("dbo.CustomerOrders", "DeleteBillDate");
            DropColumn("dbo.CustomerOrders", "DeleteReason");
        }
    }
}
