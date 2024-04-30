namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class odg_ProductTransfertLine : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductTransfertLines", "OeilDroiteGauche", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductTransfertLines", "OeilDroiteGauche");
        }
    }
}
