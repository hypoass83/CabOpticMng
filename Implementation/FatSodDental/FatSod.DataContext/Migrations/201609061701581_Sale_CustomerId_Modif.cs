namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sale_CustomerId_Modif : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Sales", new[] { "CustomerID" });
            AlterColumn("dbo.Sales", "CustomerID", c => c.Int());
            CreateIndex("dbo.Sales", "CustomerID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Sales", new[] { "CustomerID" });
            AlterColumn("dbo.Sales", "CustomerID", c => c.Int(nullable: false));
            CreateIndex("dbo.Sales", "CustomerID");
        }
    }
}
