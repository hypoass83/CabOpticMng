namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isStockControlField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserConfigurations", "isStockControl", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserConfigurations", "isStockControl");
        }
    }
}
