namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer_AssureurID_null : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Customers", new[] { "AssureurID" });
            AlterColumn("dbo.Customers", "AssureurID", c => c.Int());
            CreateIndex("dbo.Customers", "AssureurID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Customers", new[] { "AssureurID" });
            AlterColumn("dbo.Customers", "AssureurID", c => c.Int(nullable: false));
            CreateIndex("dbo.Customers", "AssureurID");
        }
    }
}
