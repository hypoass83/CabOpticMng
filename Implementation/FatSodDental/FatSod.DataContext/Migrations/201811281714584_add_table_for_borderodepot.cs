namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_table_for_borderodepot : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BorderoDepots",
                c => new
                    {
                        BorderoDepotID = c.Int(nullable: false, identity: true),
                        BorderoDepotDate = c.DateTime(nullable: false),
                        CodeBorderoDepot = c.String(nullable: false, maxLength: 500),
                        ValideBorderoDepot = c.Boolean(nullable: false),
                        ValidBorderoDepotDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.BorderoDepotID)
                .Index(t => t.CodeBorderoDepot, unique: true);
            
            AddColumn("dbo.CustomerOrders", "BorderoDepotID", c => c.Int());
            CreateIndex("dbo.CustomerOrders", "BorderoDepotID");
            AddForeignKey("dbo.CustomerOrders", "BorderoDepotID", "dbo.BorderoDepots", "BorderoDepotID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerOrders", "BorderoDepotID", "dbo.BorderoDepots");
            DropIndex("dbo.BorderoDepots", new[] { "CodeBorderoDepot" });
            DropIndex("dbo.CustomerOrders", new[] { "BorderoDepotID" });
            DropColumn("dbo.CustomerOrders", "BorderoDepotID");
            DropTable("dbo.BorderoDepots");
        }
    }
}
