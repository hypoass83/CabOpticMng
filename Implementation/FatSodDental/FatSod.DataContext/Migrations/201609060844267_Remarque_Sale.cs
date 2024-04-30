namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remarque_Sale : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "Remarque", c => c.String());
            AddColumn("dbo.Sales", "MedecinTraitant", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sales", "MedecinTraitant");
            DropColumn("dbo.Sales", "Remarque");
        }
    }
}
