namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_BorderoDepot_Entity_Heure_user : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BorderoDepots", "HeureGenerateBordero", c => c.String());
            AddColumn("dbo.BorderoDepots", "GenerateByID", c => c.Int());
            AddColumn("dbo.BorderoDepots", "HeureValidateBordero", c => c.String());
            AddColumn("dbo.BorderoDepots", "ValidateByID", c => c.Int());
            CreateIndex("dbo.BorderoDepots", "GenerateByID");
            CreateIndex("dbo.BorderoDepots", "ValidateByID");
            AddForeignKey("dbo.BorderoDepots", "GenerateByID", "dbo.Users", "GlobalPersonID");
            AddForeignKey("dbo.BorderoDepots", "ValidateByID", "dbo.Users", "GlobalPersonID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BorderoDepots", "ValidateByID", "dbo.Users");
            DropForeignKey("dbo.BorderoDepots", "GenerateByID", "dbo.Users");
            DropIndex("dbo.BorderoDepots", new[] { "ValidateByID" });
            DropIndex("dbo.BorderoDepots", new[] { "GenerateByID" });
            DropColumn("dbo.BorderoDepots", "ValidateByID");
            DropColumn("dbo.BorderoDepots", "HeureValidateBordero");
            DropColumn("dbo.BorderoDepots", "GenerateByID");
            DropColumn("dbo.BorderoDepots", "HeureGenerateBordero");
        }
    }
}
