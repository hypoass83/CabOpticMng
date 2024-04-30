namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerOrderLine_Add_FrameCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrderLines", "FrameCategory", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerOrderLines", "FrameCategory");
        }
    }
}
