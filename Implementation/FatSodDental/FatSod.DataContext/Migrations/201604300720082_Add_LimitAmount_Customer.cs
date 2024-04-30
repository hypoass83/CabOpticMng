namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_LimitAmount_Customer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "GestionnaireID", c => c.Int());
            AddColumn("dbo.Customers", "LimitAmount", c => c.Double(nullable: false));
            CreateIndex("dbo.Customers", "GestionnaireID");
            AddForeignKey("dbo.Customers", "GestionnaireID", "dbo.Users", "GlobalPersonID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Customers", "GestionnaireID", "dbo.Users");
            DropIndex("dbo.Customers", new[] { "GestionnaireID" });
            DropColumn("dbo.Customers", "LimitAmount");
            DropColumn("dbo.Customers", "GestionnaireID");
        }
    }
}
