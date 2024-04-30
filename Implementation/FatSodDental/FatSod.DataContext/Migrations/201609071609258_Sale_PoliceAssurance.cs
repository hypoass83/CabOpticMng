namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sale_PoliceAssurance : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "PoliceAssurance", c => c.String());
            DropColumn("dbo.Sales", "Representant");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sales", "Representant", c => c.String());
            DropColumn("dbo.Sales", "PoliceAssurance");
        }
    }
}
