namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerOrder_Plafond : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrders", "Plafond", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerOrders", "Plafond");
        }
    }
}
