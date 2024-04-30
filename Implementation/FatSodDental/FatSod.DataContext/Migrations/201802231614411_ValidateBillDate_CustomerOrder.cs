namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ValidateBillDate_CustomerOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrders", "ValidateBillDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerOrders", "ValidateBillDate");
        }
    }
}
