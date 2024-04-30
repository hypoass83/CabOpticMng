namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Table_ProductDamage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductDamageLines",
                c => new
                    {
                        ProductDamageLineID = c.Int(nullable: false, identity: true),
                        ProductID = c.Int(nullable: false),
                        LineQuantity = c.Double(nullable: false),
                        LineUnitPrice = c.Double(nullable: false),
                        LocalizationID = c.Int(nullable: false),
                        ProductDamageID = c.Int(nullable: false),
                        OeilDroiteGauche = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductDamageLineID)
                .ForeignKey("dbo.Localizations", t => t.LocalizationID)
                .ForeignKey("dbo.Products", t => t.ProductID)
                .ForeignKey("dbo.ProductDamages", t => t.ProductDamageID)
                .Index(t => t.ProductID)
                .Index(t => t.LocalizationID)
                .Index(t => t.ProductDamageID);
            
            CreateTable(
                "dbo.ProductDamages",
                c => new
                    {
                        ProductDamageID = c.Int(nullable: false, identity: true),
                        ProductDamageDate = c.DateTime(nullable: false),
                        ProductDamageReference = c.String(nullable: false, maxLength: 50),
                        BranchID = c.Int(nullable: false),
                        AutorizedByID = c.Int(),
                        RegisteredByID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductDamageID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .ForeignKey("dbo.Users", t => t.AutorizedByID)
                .ForeignKey("dbo.Users", t => t.RegisteredByID)
                .Index(t => t.ProductDamageReference, unique: true, name: "ProductDamageReference")
                .Index(t => t.BranchID)
                .Index(t => t.AutorizedByID)
                .Index(t => t.RegisteredByID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductDamages", "RegisteredByID", "dbo.Users");
            DropForeignKey("dbo.ProductDamageLines", "ProductDamageID", "dbo.ProductDamages");
            DropForeignKey("dbo.ProductDamages", "AutorizedByID", "dbo.Users");
            DropForeignKey("dbo.ProductDamages", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.ProductDamageLines", "ProductID", "dbo.Products");
            DropForeignKey("dbo.ProductDamageLines", "LocalizationID", "dbo.Localizations");
            DropIndex("dbo.ProductDamages", new[] { "RegisteredByID" });
            DropIndex("dbo.ProductDamages", new[] { "AutorizedByID" });
            DropIndex("dbo.ProductDamages", new[] { "BranchID" });
            DropIndex("dbo.ProductDamages", "ProductDamageReference");
            DropIndex("dbo.ProductDamageLines", new[] { "ProductDamageID" });
            DropIndex("dbo.ProductDamageLines", new[] { "LocalizationID" });
            DropIndex("dbo.ProductDamageLines", new[] { "ProductID" });
            DropTable("dbo.ProductDamages");
            DropTable("dbo.ProductDamageLines");
        }
    }
}
