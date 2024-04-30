namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change_Assurance_Entity : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Assureurs", "AssureurNumber");
            DropColumn("dbo.Assureurs", "AssureurNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Assureurs", "AssureurNumber", c => c.String(maxLength: 250));
            CreateIndex("dbo.Assureurs", "AssureurNumber", unique: true, name: "AssureurNumber");
        }
    }
}
