namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modif_Sale_Compteur : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "CompteurFacture", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sales", "CompteurFacture");
        }
    }
}
