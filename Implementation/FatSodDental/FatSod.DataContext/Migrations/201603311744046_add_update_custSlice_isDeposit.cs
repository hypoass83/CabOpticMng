namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_update_custSlice_isDeposit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerSlices", "isDeposit", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerSlices", "isDeposit");
        }
    }
}
