namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_TotalMalade_CustomerOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrders", "TotalMalade", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerOrders", "TotalMalade");
        }
    }
}
