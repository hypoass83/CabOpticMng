namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class price_Indexes_2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.LensNumberRangePrices", "IX_RealPrimaryKey");
            CreateIndex("dbo.LensNumberRangePrices", "LensCategoryID");
            CreateIndex("dbo.LensNumberRangePrices", "SphericalValueRangeID");
            CreateIndex("dbo.LensNumberRangePrices", "CylindricalValueRangeID");
            CreateIndex("dbo.LensNumberRangePrices", "AdditionValueRangeID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.LensNumberRangePrices", new[] { "AdditionValueRangeID" });
            DropIndex("dbo.LensNumberRangePrices", new[] { "CylindricalValueRangeID" });
            DropIndex("dbo.LensNumberRangePrices", new[] { "SphericalValueRangeID" });
            DropIndex("dbo.LensNumberRangePrices", new[] { "LensCategoryID" });
            CreateIndex("dbo.LensNumberRangePrices", new[] { "LensCategoryID", "SphericalValueRangeID", "CylindricalValueRangeID", "AdditionValueRangeID" }, unique: true, name: "IX_RealPrimaryKey");
        }
    }
}
