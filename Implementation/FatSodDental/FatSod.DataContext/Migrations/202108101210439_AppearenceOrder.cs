namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AppearenceOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Menus", "AppearanceOrder", c => c.Int(nullable: false));
            AddColumn("dbo.Menus", "MenuStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Modules", "AppearanceOrder", c => c.Int(nullable: false));
            AddColumn("dbo.Modules", "ModuleStatus", c => c.Int(nullable: false));
            AddColumn("dbo.SubMenus", "AppearanceOrder", c => c.Int(nullable: false));
            AddColumn("dbo.SubMenus", "SubMenuStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SubMenus", "SubMenuStatus");
            DropColumn("dbo.SubMenus", "AppearanceOrder");
            DropColumn("dbo.Modules", "ModuleStatus");
            DropColumn("dbo.Modules", "AppearanceOrder");
            DropColumn("dbo.Menus", "MenuStatus");
            DropColumn("dbo.Menus", "AppearanceOrder");
        }
    }
}
