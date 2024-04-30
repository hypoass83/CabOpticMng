namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LensCategory_TypeLens : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LensCategories", "TypeLens", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LensCategories", "TypeLens");
        }
    }
}
