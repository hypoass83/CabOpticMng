namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_BorderoDepot_Entity : DbMigration
    {
        public override void Up()
        {
            
            DropIndex("dbo.BorderoDepots", new[] { "CodeBorderoDepot" });
            Sql(@"UPDATE CustomerOrders SET BorderoDepotID=NULL");
            Sql(@"DELETE FROM dbo.BorderoDepots");

            AddColumn("dbo.BorderoDepots", "AssureurID", c => c.Int(nullable: false));
            AddColumn("dbo.BorderoDepots", "CompanyID", c => c.String());
            AddColumn("dbo.BorderoDepots", "LieuxdeDepotBorderoID", c => c.Int(nullable: false));
            AlterColumn("dbo.BorderoDepots", "CodeBorderoDepot", c => c.String(nullable: false, maxLength: 50));
            CreateIndex("dbo.BorderoDepots", "AssureurID");
            AddForeignKey("dbo.BorderoDepots", "AssureurID", "dbo.Assureurs", "GlobalPersonID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BorderoDepots", "AssureurID", "dbo.Assureurs");
            DropIndex("dbo.BorderoDepots", new[] { "AssureurID" });
            AlterColumn("dbo.BorderoDepots", "CodeBorderoDepot", c => c.String(nullable: false, maxLength: 500));
            DropColumn("dbo.BorderoDepots", "LieuxdeDepotBorderoID");
            DropColumn("dbo.BorderoDepots", "CompanyID");
            DropColumn("dbo.BorderoDepots", "AssureurID");
            CreateIndex("dbo.BorderoDepots", "CodeBorderoDepot", unique: true);
        }
    }
}
