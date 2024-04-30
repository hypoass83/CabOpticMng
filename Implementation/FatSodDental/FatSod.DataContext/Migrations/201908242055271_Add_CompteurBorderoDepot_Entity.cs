namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_CompteurBorderoDepot_Entity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompteurBorderoDepots",
                c => new
                    {
                        CompteurBorderoDepotID = c.Int(nullable: false, identity: true),
                        CompteurBorderoDepotCode = c.String(maxLength: 16),
                        Counter = c.Int(nullable: false),
                        YearOperation = c.Int(nullable: false),
                        CompanyID = c.String(),
                        LieuxdeDepotBorderoID = c.Int(nullable: false),
                        AssureurID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CompteurBorderoDepotID)
                .ForeignKey("dbo.Assureurs", t => t.AssureurID)
                .Index(t => t.AssureurID);

            Sql(@"INSERT INTO dbo.CompteurBorderoDepots(CompteurBorderoDepotCode,Counter,YearOperation,CompanyID,LieuxdeDepotBorderoID,AssureurID) 
                SELECT '0000' as CompteurBorderoDepotCode,0 as Counter, Year(getdate()) as YearOperation,NUll as CompanyID,0 as LieuxdeDepotBorderoID,
                GlobalPersonID as AssureurID FROM dbo.Assureurs");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompteurBorderoDepots", "AssureurID", "dbo.Assureurs");
            DropIndex("dbo.CompteurBorderoDepots", new[] { "AssureurID" });
            DropTable("dbo.CompteurBorderoDepots");
        }
    }
}
