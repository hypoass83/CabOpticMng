namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompteurFacture_Assureur : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assureurs", "CompteurFacture", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Assureurs", "CompteurFacture");
        }
    }
}
