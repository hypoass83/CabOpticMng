namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_BarcodeImage_RptBareCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RptBareCodes", "BarcodeImage", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RptBareCodes", "BarcodeImage");
        }
    }
}
