namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StockReplacement_Entity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StockReplacementLines",
                c => new
                    {
                        StockReplacementLineID = c.Int(nullable: false, identity: true),
                        ProductID = c.Int(nullable: false),
                        LineQuantity = c.Double(nullable: false),
                        LineUnitPrice = c.Double(nullable: false),
                        LocalizationID = c.Int(nullable: false),
                        StockReplacementID = c.Int(nullable: false),
                        OeilDroiteGauche = c.Int(nullable: false),
                        StockReplacementReason = c.String(),
                        NumeroSerie = c.String(),
                        Marque = c.String(),
                        ProductDamageID = c.Int(nullable: false),
                        NumeroSerieDamage = c.String(),
                        MarqueDamage = c.String(),
                    })
                .PrimaryKey(t => t.StockReplacementLineID)
                .ForeignKey("dbo.Localizations", t => t.LocalizationID)
                .ForeignKey("dbo.Products", t => t.ProductID)
                .ForeignKey("dbo.Products", t => t.ProductDamageID)
                .ForeignKey("dbo.StockReplacements", t => t.StockReplacementID)
                .Index(t => t.ProductID)
                .Index(t => t.LocalizationID)
                .Index(t => t.StockReplacementID)
                .Index(t => t.ProductDamageID);
            
            CreateTable(
                "dbo.StockReplacements",
                c => new
                    {
                        StockReplacementID = c.Int(nullable: false, identity: true),
                        StockReplacementDate = c.DateTime(nullable: false),
                        StockReplacementReference = c.String(nullable: false, maxLength: 50),
                        BranchID = c.Int(nullable: false),
                        AutorizedByID = c.Int(),
                        RegisteredByID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StockReplacementID)
                .ForeignKey("dbo.Users", t => t.AutorizedByID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .ForeignKey("dbo.Users", t => t.RegisteredByID)
                .Index(t => t.StockReplacementReference, unique: true, name: "StockReplacementReference")
                .Index(t => t.BranchID)
                .Index(t => t.AutorizedByID)
                .Index(t => t.RegisteredByID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StockReplacementLines", "StockReplacementID", "dbo.StockReplacements");
            DropForeignKey("dbo.StockReplacements", "RegisteredByID", "dbo.Users");
            DropForeignKey("dbo.StockReplacements", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.StockReplacements", "AutorizedByID", "dbo.Users");
            DropForeignKey("dbo.StockReplacementLines", "ProductDamageID", "dbo.Products");
            DropForeignKey("dbo.StockReplacementLines", "ProductID", "dbo.Products");
            DropForeignKey("dbo.StockReplacementLines", "LocalizationID", "dbo.Localizations");
            DropIndex("dbo.StockReplacements", new[] { "RegisteredByID" });
            DropIndex("dbo.StockReplacements", new[] { "AutorizedByID" });
            DropIndex("dbo.StockReplacements", new[] { "BranchID" });
            DropIndex("dbo.StockReplacements", "StockReplacementReference");
            DropIndex("dbo.StockReplacementLines", new[] { "ProductDamageID" });
            DropIndex("dbo.StockReplacementLines", new[] { "StockReplacementID" });
            DropIndex("dbo.StockReplacementLines", new[] { "LocalizationID" });
            DropIndex("dbo.StockReplacementLines", new[] { "ProductID" });
            DropTable("dbo.StockReplacements");
            DropTable("dbo.StockReplacementLines");
        }
    }
}
