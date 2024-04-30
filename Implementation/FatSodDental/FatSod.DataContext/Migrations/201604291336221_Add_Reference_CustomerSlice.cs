namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Reference_CustomerSlice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerSlices", "Reference", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerSlices", "Reference");
        }
    }
}
