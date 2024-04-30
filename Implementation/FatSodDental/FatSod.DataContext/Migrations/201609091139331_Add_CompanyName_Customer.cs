namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_CompanyName_Customer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "CompanyName", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "CompanyName");
        }
    }
}
