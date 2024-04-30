namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_GestionnaireID_custorder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrders", "GestionnaireID", c => c.Int());
            CreateIndex("dbo.CustomerOrders", "GestionnaireID");
            AddForeignKey("dbo.CustomerOrders", "GestionnaireID", "dbo.Users", "GlobalPersonID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerOrders", "GestionnaireID", "dbo.Users");
            DropIndex("dbo.CustomerOrders", new[] { "GestionnaireID" });
            DropColumn("dbo.CustomerOrders", "GestionnaireID");
        }
    }
}
