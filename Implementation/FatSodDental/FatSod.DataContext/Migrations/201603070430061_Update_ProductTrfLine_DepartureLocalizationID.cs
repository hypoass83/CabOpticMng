namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_ProductTrfLine_DepartureLocalizationID : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProductTransfertLines", new[] { "DepartureLocalizationID" });
            AlterColumn("dbo.ProductTransfertLines", "DepartureLocalizationID", c => c.Int());
            CreateIndex("dbo.ProductTransfertLines", "DepartureLocalizationID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ProductTransfertLines", new[] { "DepartureLocalizationID" });
            AlterColumn("dbo.ProductTransfertLines", "DepartureLocalizationID", c => c.Int(nullable: false));
            CreateIndex("dbo.ProductTransfertLines", "DepartureLocalizationID");
        }
    }
}
