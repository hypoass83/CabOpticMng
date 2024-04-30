namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserConfig_isTellerControl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserConfigurations", "isTellerControl", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserConfigurations", "isTellerControl");
        }
    }
}
