namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Table_Statuts_State : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TillDayStatus", "IsOpen", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TillDayStatus", "IsOpen");
        }
    }
}
