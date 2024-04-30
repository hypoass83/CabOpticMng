namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Dateregister_customer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "Dateregister", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "Dateregister");
        }
    }
}
