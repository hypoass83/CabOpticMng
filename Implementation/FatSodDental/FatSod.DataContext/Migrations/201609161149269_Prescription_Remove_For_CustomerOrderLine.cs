namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Prescription_Remove_For_CustomerOrderLine : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.CustomerOrderLines", "ProductCategoryCode");
            DropColumn("dbo.CustomerOrderLines", "Prescription");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CustomerOrderLines", "Prescription", c => c.String());
            AddColumn("dbo.CustomerOrderLines", "ProductCategoryCode", c => c.String());
        }
    }
}
