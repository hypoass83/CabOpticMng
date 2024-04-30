namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProductTransfert : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductTransfertLines", "ArrivalLocalizationID", "dbo.Localizations");
            DropForeignKey("dbo.ProductTransfertLines", "DepartureLocalizationID", "dbo.Localizations");
            DropForeignKey("dbo.ProductTransfertLines", "ProductID", "dbo.Products");
            DropIndex("dbo.ProductTransferts", new[] { "DepartureBranchID" });
            DropIndex("dbo.ProductTransferts", new[] { "ArrivalBranchID" });
            DropIndex("dbo.ProductTransferts", new[] { "AskedByID" });
            DropIndex("dbo.ProductTransferts", new[] { "RegisteredByID" });
            DropIndex("dbo.ProductTransferts", new[] { "ReceivedByID" });
            DropIndex("dbo.ProductTransfertLines", new[] { "ProductID" });
            DropIndex("dbo.ProductTransfertLines", new[] { "DepartureLocalizationID" });
            DropIndex("dbo.ProductTransfertLines", new[] { "ArrivalLocalizationID" });
            DropIndex("dbo.ProductTransfertLines", new[] { "ProductTransfertID" });
            AddColumn("dbo.ProductTransfertLines", "DepartureStockId", c => c.Int());
            AddColumn("dbo.ProductTransfertLines", "ArrivalStockId", c => c.Int());
            AddColumn("dbo.ProductTransfertLines", "Quantity", c => c.Double(nullable: false));
            AddColumn("dbo.ProductTransfertLines", "UnitPrice", c => c.Double(nullable: false));
            CreateIndex("dbo.ProductTransferts", "DepartureBranchId");
            CreateIndex("dbo.ProductTransferts", "ArrivalBranchId");
            CreateIndex("dbo.ProductTransferts", "AskedById");
            CreateIndex("dbo.ProductTransferts", "RegisteredById");
            CreateIndex("dbo.ProductTransferts", "ReceivedById");
            CreateIndex("dbo.ProductTransfertLines", "DepartureStockId");
            CreateIndex("dbo.ProductTransfertLines", "ArrivalStockId");
            CreateIndex("dbo.ProductTransfertLines", "ProductTransfertId");
            AddForeignKey("dbo.ProductTransfertLines", "ArrivalStockId", "dbo.ProductLocalizations", "ProductLocalizationID");
            AddForeignKey("dbo.ProductTransfertLines", "DepartureStockId", "dbo.ProductLocalizations", "ProductLocalizationID");
            DropColumn("dbo.ProductTransfertLines", "ProductID");
            DropColumn("dbo.ProductTransfertLines", "LineQuantity");
            DropColumn("dbo.ProductTransfertLines", "LineUnitPrice");
            DropColumn("dbo.ProductTransfertLines", "DepartureLocalizationID");
            DropColumn("dbo.ProductTransfertLines", "ArrivalLocalizationID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductTransfertLines", "ArrivalLocalizationID", c => c.Int());
            AddColumn("dbo.ProductTransfertLines", "DepartureLocalizationID", c => c.Int());
            AddColumn("dbo.ProductTransfertLines", "LineUnitPrice", c => c.Double(nullable: false));
            AddColumn("dbo.ProductTransfertLines", "LineQuantity", c => c.Double(nullable: false));
            AddColumn("dbo.ProductTransfertLines", "ProductID", c => c.Int(nullable: false));
            DropForeignKey("dbo.ProductTransfertLines", "DepartureStockId", "dbo.ProductLocalizations");
            DropForeignKey("dbo.ProductTransfertLines", "ArrivalStockId", "dbo.ProductLocalizations");
            DropIndex("dbo.ProductTransfertLines", new[] { "ProductTransfertId" });
            DropIndex("dbo.ProductTransfertLines", new[] { "ArrivalStockId" });
            DropIndex("dbo.ProductTransfertLines", new[] { "DepartureStockId" });
            DropIndex("dbo.ProductTransferts", new[] { "ReceivedById" });
            DropIndex("dbo.ProductTransferts", new[] { "RegisteredById" });
            DropIndex("dbo.ProductTransferts", new[] { "AskedById" });
            DropIndex("dbo.ProductTransferts", new[] { "ArrivalBranchId" });
            DropIndex("dbo.ProductTransferts", new[] { "DepartureBranchId" });
            DropColumn("dbo.ProductTransfertLines", "UnitPrice");
            DropColumn("dbo.ProductTransfertLines", "Quantity");
            DropColumn("dbo.ProductTransfertLines", "ArrivalStockId");
            DropColumn("dbo.ProductTransfertLines", "DepartureStockId");
            CreateIndex("dbo.ProductTransfertLines", "ProductTransfertID");
            CreateIndex("dbo.ProductTransfertLines", "ArrivalLocalizationID");
            CreateIndex("dbo.ProductTransfertLines", "DepartureLocalizationID");
            CreateIndex("dbo.ProductTransfertLines", "ProductID");
            CreateIndex("dbo.ProductTransferts", "ReceivedByID");
            CreateIndex("dbo.ProductTransferts", "RegisteredByID");
            CreateIndex("dbo.ProductTransferts", "AskedByID");
            CreateIndex("dbo.ProductTransferts", "ArrivalBranchID");
            CreateIndex("dbo.ProductTransferts", "DepartureBranchID");
            AddForeignKey("dbo.ProductTransfertLines", "ProductID", "dbo.Products", "ProductID");
            AddForeignKey("dbo.ProductTransfertLines", "DepartureLocalizationID", "dbo.Localizations", "LocalizationID");
            AddForeignKey("dbo.ProductTransfertLines", "ArrivalLocalizationID", "dbo.Localizations", "LocalizationID");
        }
    }
}
