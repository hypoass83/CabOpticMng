namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_SerialNumber_Properties : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductBrands",
                c => new
                    {
                        ProductBrandID = c.Int(nullable: false, identity: true),
                        ProductBrandCode = c.String(nullable: false, maxLength: 100),
                        ProductBrandLabel = c.String(),
                    })
                .PrimaryKey(t => t.ProductBrandID)
                .Index(t => t.ProductBrandCode, unique: true, name: "ProductBrandCode");
            
            AddColumn("dbo.Lines", "NumeroSerie", c => c.String());
            AddColumn("dbo.ProductLocalizations", "NumeroSerie", c => c.String());
            AddColumn("dbo.ProductLocalizations", "Marque", c => c.String());
            AddColumn("dbo.ProductLocalizations", "ProductBrand_ProductBrandID", c => c.Int());
            AddColumn("dbo.Categories", "isSerialNumberNull", c => c.Boolean(nullable: false));
            AddColumn("dbo.InventoryDirectoryLines", "NumeroSerie", c => c.String());
            AddColumn("dbo.InventoryDirectoryLines", "Marque", c => c.String());
            AddColumn("dbo.InventoryHistorics", "NumeroSerie", c => c.String());
            AddColumn("dbo.InventoryHistorics", "Marque", c => c.String());
            AddColumn("dbo.ProductDamageLines", "NumeroSerie", c => c.String());
            AddColumn("dbo.ProductDamageLines", "Marque", c => c.String());
            AddColumn("dbo.ProductGiftLines", "NumeroSerie", c => c.String());
            AddColumn("dbo.ProductGiftLines", "Marque", c => c.String());
            CreateIndex("dbo.ProductLocalizations", "ProductBrand_ProductBrandID");
            AddForeignKey("dbo.ProductLocalizations", "ProductBrand_ProductBrandID", "dbo.ProductBrands", "ProductBrandID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductLocalizations", "ProductBrand_ProductBrandID", "dbo.ProductBrands");
            DropIndex("dbo.ProductBrands", "ProductBrandCode");
            DropIndex("dbo.ProductLocalizations", new[] { "ProductBrand_ProductBrandID" });
            DropColumn("dbo.ProductGiftLines", "Marque");
            DropColumn("dbo.ProductGiftLines", "NumeroSerie");
            DropColumn("dbo.ProductDamageLines", "Marque");
            DropColumn("dbo.ProductDamageLines", "NumeroSerie");
            DropColumn("dbo.InventoryHistorics", "Marque");
            DropColumn("dbo.InventoryHistorics", "NumeroSerie");
            DropColumn("dbo.InventoryDirectoryLines", "Marque");
            DropColumn("dbo.InventoryDirectoryLines", "NumeroSerie");
            DropColumn("dbo.Categories", "isSerialNumberNull");
            DropColumn("dbo.ProductLocalizations", "ProductBrand_ProductBrandID");
            DropColumn("dbo.ProductLocalizations", "Marque");
            DropColumn("dbo.ProductLocalizations", "NumeroSerie");
            DropColumn("dbo.Lines", "NumeroSerie");
            DropTable("dbo.ProductBrands");
        }
    }
}
