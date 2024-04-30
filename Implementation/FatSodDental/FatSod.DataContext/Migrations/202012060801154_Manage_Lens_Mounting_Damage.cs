namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Manage_Lens_Mounting_Damage : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProductDamages", new[] { "RegisteredByID" });
            AddColumn("dbo.ProductDamageLines", "CumulSaleAndBillLineID", c => c.Int());
            AddColumn("dbo.ProductDamages", "IsLensMountingDamage", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductDamages", "IsStockOutPut", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductDamages", "CumulSaleAndBillID", c => c.Int());
            AlterColumn("dbo.ProductDamages", "RegisteredByID", c => c.Int());
            CreateIndex("dbo.ProductDamageLines", "CumulSaleAndBillLineID");
            CreateIndex("dbo.ProductDamages", "RegisteredByID");
            CreateIndex("dbo.ProductDamages", "CumulSaleAndBillID");
            AddForeignKey("dbo.ProductDamageLines", "CumulSaleAndBillLineID", "dbo.CumulSaleAndBillLines", "LineID");
            AddForeignKey("dbo.ProductDamages", "CumulSaleAndBillID", "dbo.CumulSaleAndBills", "CumulSaleAndBillID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductDamages", "CumulSaleAndBillID", "dbo.CumulSaleAndBills");
            DropForeignKey("dbo.ProductDamageLines", "CumulSaleAndBillLineID", "dbo.CumulSaleAndBillLines");
            DropIndex("dbo.ProductDamages", new[] { "CumulSaleAndBillID" });
            DropIndex("dbo.ProductDamages", new[] { "RegisteredByID" });
            DropIndex("dbo.ProductDamageLines", new[] { "CumulSaleAndBillLineID" });
            AlterColumn("dbo.ProductDamages", "RegisteredByID", c => c.Int(nullable: false));
            DropColumn("dbo.ProductDamages", "CumulSaleAndBillID");
            DropColumn("dbo.ProductDamages", "IsStockOutPut");
            DropColumn("dbo.ProductDamages", "IsLensMountingDamage");
            DropColumn("dbo.ProductDamageLines", "CumulSaleAndBillLineID");
            CreateIndex("dbo.ProductDamages", "RegisteredByID");
        }
    }
}
