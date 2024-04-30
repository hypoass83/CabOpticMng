namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SpecialOrderCustomerOrderLine : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SpecialOrders", "PostedToSupplierDate", c => c.DateTime());
            AddColumn("dbo.CustomerOrderLines", "SpecialOrderLineCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerOrderLines", "SpecialOrderLineCode");
            DropColumn("dbo.SpecialOrders", "PostedToSupplierDate");
        }
    }
}
