namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerOrder_MntReduction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrders", "MntReduction", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerOrders", "MntReduction");
        }
    }
}
