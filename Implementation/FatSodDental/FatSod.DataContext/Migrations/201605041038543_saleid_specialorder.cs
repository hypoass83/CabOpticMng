namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class saleid_specialorder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SpecialOrders", "SaleID", c => c.Int());
            CreateIndex("dbo.SpecialOrders", "SaleID");
            AddForeignKey("dbo.SpecialOrders", "SaleID", "dbo.Sales", "SaleID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SpecialOrders", "SaleID", "dbo.Sales");
            DropIndex("dbo.SpecialOrders", new[] { "SaleID" });
            DropColumn("dbo.SpecialOrders", "SaleID");
        }
    }
}
