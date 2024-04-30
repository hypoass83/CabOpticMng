namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modif_Sale_SaleDateHours : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "SaleDateHours", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sales", "SaleDateHours");
        }
    }
}
