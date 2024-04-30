namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_IsSpecialOrder_Sale : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "IsSpecialOrder", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sales", "IsSpecialOrder");
        }
    }
}
