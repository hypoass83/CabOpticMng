namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_CompteurCodeBar_Barcodegenerator : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BarCodeGenerators", "CompteurCodeBar", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BarCodeGenerators", "CompteurCodeBar");
        }
    }
}
