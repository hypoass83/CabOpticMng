namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_IsSpecialOrder_AllDeposit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AllDeposits", "IsSpecialOrder", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AllDeposits", "IsSpecialOrder");
        }
    }
}
