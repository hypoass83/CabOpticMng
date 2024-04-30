namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change_Customer_Entity_id : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Customers", "CustomerNumber");
            DropColumn("dbo.Customers", "CustomerNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Customers", "CustomerNumber", c => c.String(maxLength: 250));
            CreateIndex("dbo.Customers", "CustomerNumber", unique: true, name: "CustomerNumber");
        }
    }
}
