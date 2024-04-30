namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Table_StatutsTill_Gist : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductGiftLines",
                c => new
                    {
                        ProductGiftLineID = c.Int(nullable: false, identity: true),
                        ProductID = c.Int(nullable: false),
                        LineQuantity = c.Double(nullable: false),
                        LineUnitPrice = c.Double(nullable: false),
                        LocalizationID = c.Int(nullable: false),
                        ProductGiftID = c.Int(nullable: false),
                        OeilDroiteGauche = c.Int(nullable: false),
                        ProductGiftReason = c.String(),
                    })
                .PrimaryKey(t => t.ProductGiftLineID)
                .ForeignKey("dbo.Localizations", t => t.LocalizationID)
                .ForeignKey("dbo.Products", t => t.ProductID)
                .ForeignKey("dbo.ProductGifts", t => t.ProductGiftID)
                .Index(t => t.ProductID)
                .Index(t => t.LocalizationID)
                .Index(t => t.ProductGiftID);
            
            CreateTable(
                "dbo.ProductGifts",
                c => new
                    {
                        ProductGiftID = c.Int(nullable: false, identity: true),
                        ProductGiftDate = c.DateTime(nullable: false),
                        ProductGiftReference = c.String(nullable: false, maxLength: 50),
                        BranchID = c.Int(nullable: false),
                        AutorizedByID = c.Int(),
                        RegisteredByID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductGiftID)
                .ForeignKey("dbo.Users", t => t.AutorizedByID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .ForeignKey("dbo.Users", t => t.RegisteredByID)
                .Index(t => t.ProductGiftReference, unique: true, name: "ProductGiftReference")
                .Index(t => t.BranchID)
                .Index(t => t.AutorizedByID)
                .Index(t => t.RegisteredByID);
            
            CreateTable(
                "dbo.TillDayStatus",
                c => new
                    {
                        TillDayStatusID = c.Int(nullable: false, identity: true),
                        TillDayLastOpenDate = c.DateTime(nullable: false),
                        TillDayLastClosingDate = c.DateTime(nullable: false),
                        TillID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TillDayStatusID)
                .ForeignKey("dbo.Tills", t => t.TillID)
                .Index(t => t.TillID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TillDayStatus", "TillID", "dbo.Tills");
            DropForeignKey("dbo.ProductGifts", "RegisteredByID", "dbo.Users");
            DropForeignKey("dbo.ProductGiftLines", "ProductGiftID", "dbo.ProductGifts");
            DropForeignKey("dbo.ProductGifts", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.ProductGifts", "AutorizedByID", "dbo.Users");
            DropForeignKey("dbo.ProductGiftLines", "ProductID", "dbo.Products");
            DropForeignKey("dbo.ProductGiftLines", "LocalizationID", "dbo.Localizations");
            DropIndex("dbo.TillDayStatus", new[] { "TillID" });
            DropIndex("dbo.ProductGifts", new[] { "RegisteredByID" });
            DropIndex("dbo.ProductGifts", new[] { "AutorizedByID" });
            DropIndex("dbo.ProductGifts", new[] { "BranchID" });
            DropIndex("dbo.ProductGifts", "ProductGiftReference");
            DropIndex("dbo.ProductGiftLines", new[] { "ProductGiftID" });
            DropIndex("dbo.ProductGiftLines", new[] { "LocalizationID" });
            DropIndex("dbo.ProductGiftLines", new[] { "ProductID" });
            DropTable("dbo.TillDayStatus");
            DropTable("dbo.ProductGifts");
            DropTable("dbo.ProductGiftLines");
        }
    }
}
