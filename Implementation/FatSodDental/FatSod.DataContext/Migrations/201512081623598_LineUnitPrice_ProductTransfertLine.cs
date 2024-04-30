namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LineUnitPrice_ProductTransfertLine : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductTransfertLines", "LineUnitPrice", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductTransfertLines", "LineUnitPrice");
        }
    }
}
