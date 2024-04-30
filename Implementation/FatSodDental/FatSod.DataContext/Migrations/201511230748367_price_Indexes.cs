namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class price_Indexes : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.LensNumberRangePrices", new[] { "LensCategoryID" });
            DropIndex("dbo.LensNumberRangePrices", new[] { "SphericalValueRangeID" });
            DropIndex("dbo.LensNumberRangePrices", new[] { "CylindricalValueRangeID" });
            DropIndex("dbo.LensNumberRangePrices", new[] { "AdditionValueRangeID" });
            CreateIndex("dbo.LensNumberRangePrices", new[] { "LensCategoryID", "SphericalValueRangeID", "CylindricalValueRangeID", "AdditionValueRangeID" }, unique: true, name: "IX_RealPrimaryKey");
        }
        
        public override void Down()
        {
            DropIndex("dbo.LensNumberRangePrices", "IX_RealPrimaryKey");
            CreateIndex("dbo.LensNumberRangePrices", "AdditionValueRangeID");
            CreateIndex("dbo.LensNumberRangePrices", "CylindricalValueRangeID");
            CreateIndex("dbo.LensNumberRangePrices", "SphericalValueRangeID");
            CreateIndex("dbo.LensNumberRangePrices", "LensCategoryID");
        }
    }
}
