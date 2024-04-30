namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change_Assurance_Entity_ID : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Assureurs", "AssureurID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Assureurs", "AssureurID", c => c.Int(nullable: false));
        }
    }
}
