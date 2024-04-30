namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_CustomerNumber_Customer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "CustomerNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "CustomerNumber");
        }
    }
}
