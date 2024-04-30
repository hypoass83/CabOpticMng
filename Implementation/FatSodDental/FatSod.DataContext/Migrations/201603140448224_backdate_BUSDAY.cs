namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class backdate_BUSDAY : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessDays", "BackDateOperation", c => c.DateTime(nullable: false));
            AddColumn("dbo.BusinessDays", "BackDStatut", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessDays", "BackDStatut");
            DropColumn("dbo.BusinessDays", "BackDateOperation");
        }
    }
}
