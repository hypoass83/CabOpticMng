namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class index_diametre_specialOrder_cat : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LensCategories", "LensIndex", c => c.String(maxLength: 10));
            AddColumn("dbo.LensCategories", "LensDiameter", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LensCategories", "LensDiameter");
            DropColumn("dbo.LensCategories", "LensIndex");
        }
    }
}
