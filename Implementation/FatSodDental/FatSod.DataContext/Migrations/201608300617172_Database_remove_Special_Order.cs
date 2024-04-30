namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Database_remove_Special_Order : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderLenses", "ProductID", "dbo.Products");
            DropForeignKey("dbo.OrderLenses", "LensCategoryID", "dbo.LensCategories");
            DropForeignKey("dbo.OrderLenses", "LensNumberID", "dbo.LensNumbers");
            DropForeignKey("dbo.SpecialOrders", "CustomerOrderID", "dbo.CustomerOrders");
            DropForeignKey("dbo.SpecialOrders", "SaleID", "dbo.Sales");
            DropForeignKey("dbo.SpecialOrderSlices", "SliceID", "dbo.Slices");
            DropForeignKey("dbo.SpecialOrderSlices", "SpecialOrderID", "dbo.SpecialOrders");
            DropIndex("dbo.OrderLenses", new[] { "ProductID" });
            DropIndex("dbo.OrderLenses", new[] { "LensCategoryID" });
            DropIndex("dbo.OrderLenses", new[] { "LensNumberID" });
            DropIndex("dbo.SpecialOrders", new[] { "CustomerOrderID" });
            DropIndex("dbo.SpecialOrders", new[] { "Code" });
            DropIndex("dbo.SpecialOrders", new[] { "SaleID" });
            DropIndex("dbo.SpecialOrderSlices", new[] { "SliceID" });
            DropIndex("dbo.SpecialOrderSlices", new[] { "SpecialOrderID" });
            AddColumn("dbo.SaleLines", "SpecialOrderLineCode", c => c.String());
            AddColumn("dbo.SaleLines", "SupplyingName", c => c.String());
            DropTable("dbo.OrderLenses");
            DropTable("dbo.SpecialOrders");
            DropTable("dbo.SpecialOrderSlices");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SpecialOrderSlices",
                c => new
                    {
                        SliceID = c.Int(nullable: false),
                        SpecialOrderID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SliceID);
            
            CreateTable(
                "dbo.SpecialOrders",
                c => new
                    {
                        CustomerOrderID = c.Int(nullable: false),
                        Code = c.Int(nullable: false),
                        OrderStatut = c.Int(nullable: false),
                        ValidatedDate = c.DateTime(),
                        PostedToSupplierDate = c.DateTime(),
                        ReceivedDate = c.DateTime(),
                        DeliveredDate = c.DateTime(),
                        SaleID = c.Int(),
                    })
                .PrimaryKey(t => t.CustomerOrderID);
            
            CreateTable(
                "dbo.OrderLenses",
                c => new
                    {
                        ProductID = c.Int(nullable: false),
                        EyeSide = c.Int(nullable: false),
                        Addition = c.String(),
                        Axis = c.String(),
                        Index = c.String(),
                        LensCategoryID = c.Int(nullable: false),
                        LensNumberID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductID);
            
            DropColumn("dbo.SaleLines", "SupplyingName");
            DropColumn("dbo.SaleLines", "SpecialOrderLineCode");
            CreateIndex("dbo.SpecialOrderSlices", "SpecialOrderID");
            CreateIndex("dbo.SpecialOrderSlices", "SliceID");
            CreateIndex("dbo.SpecialOrders", "SaleID");
            CreateIndex("dbo.SpecialOrders", "Code", unique: true);
            CreateIndex("dbo.SpecialOrders", "CustomerOrderID");
            CreateIndex("dbo.OrderLenses", "LensNumberID");
            CreateIndex("dbo.OrderLenses", "LensCategoryID");
            CreateIndex("dbo.OrderLenses", "ProductID");
            AddForeignKey("dbo.SpecialOrderSlices", "SpecialOrderID", "dbo.SpecialOrders", "CustomerOrderID");
            AddForeignKey("dbo.SpecialOrderSlices", "SliceID", "dbo.Slices", "SliceID");
            AddForeignKey("dbo.SpecialOrders", "SaleID", "dbo.Sales", "SaleID");
            AddForeignKey("dbo.SpecialOrders", "CustomerOrderID", "dbo.CustomerOrders", "CustomerOrderID");
            AddForeignKey("dbo.OrderLenses", "LensNumberID", "dbo.LensNumbers", "LensNumberID");
            AddForeignKey("dbo.OrderLenses", "LensCategoryID", "dbo.LensCategories", "CategoryID");
            AddForeignKey("dbo.OrderLenses", "ProductID", "dbo.Products", "ProductID");
        }
    }
}
