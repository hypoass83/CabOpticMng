namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Line_IsGift : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lines", "isGift", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Lines", "isGift");
        }
    }
}
