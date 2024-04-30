namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class customerOrder_MntValidate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrders", "MntValidate", c => c.Double(nullable: false));
            DropColumn("dbo.CustomerOrders", "MntReduction");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CustomerOrders", "MntReduction", c => c.Double(nullable: false));
            DropColumn("dbo.CustomerOrders", "MntValidate");
        }
    }
}
