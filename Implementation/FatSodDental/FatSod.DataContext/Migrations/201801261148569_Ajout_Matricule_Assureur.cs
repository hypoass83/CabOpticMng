namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ajout_Matricule_Assureur : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assureurs", "Matricule", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Assureurs", "Matricule");
        }
    }
}
