namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Field_isDownloadRpt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserConfigurations", "isDownloadRpt", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserConfigurations", "isDownloadRpt");
        }
    }
}
