namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InventoryDirectoryAuthorizedById : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryDirectories", "AutorizedByID", c => c.Int());
            CreateIndex("dbo.InventoryDirectories", "AutorizedByID");
            AddForeignKey("dbo.InventoryDirectories", "AutorizedByID", "dbo.Users", "GlobalPersonID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InventoryDirectories", "AutorizedByID", "dbo.Users");
            DropIndex("dbo.InventoryDirectories", new[] { "AutorizedByID" });
            DropColumn("dbo.InventoryDirectories", "AutorizedByID");
        }
    }
}
