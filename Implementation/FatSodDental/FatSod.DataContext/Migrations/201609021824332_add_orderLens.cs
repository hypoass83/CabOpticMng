namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_orderLens : DbMigration
    {
        public override void Up()
        {
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
                .PrimaryKey(t => t.ProductID)
                .ForeignKey("dbo.Products", t => t.ProductID)
                .ForeignKey("dbo.LensCategories", t => t.LensCategoryID)
                .ForeignKey("dbo.LensNumbers", t => t.LensNumberID)
                .Index(t => t.ProductID)
                .Index(t => t.LensCategoryID)
                .Index(t => t.LensNumberID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderLenses", "LensNumberID", "dbo.LensNumbers");
            DropForeignKey("dbo.OrderLenses", "LensCategoryID", "dbo.LensCategories");
            DropForeignKey("dbo.OrderLenses", "ProductID", "dbo.Products");
            DropIndex("dbo.OrderLenses", new[] { "LensNumberID" });
            DropIndex("dbo.OrderLenses", new[] { "LensCategoryID" });
            DropIndex("dbo.OrderLenses", new[] { "ProductID" });
            DropTable("dbo.OrderLenses");
        }
    }
}
