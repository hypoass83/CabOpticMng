namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PofilLevel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "PofilLevel", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Profiles", "PofilLevel");
        }
    }
}
