namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer_order_line_Rmq_Medecin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrders", "Remarque", c => c.String());
            AddColumn("dbo.CustomerOrders", "MedecinTraitant", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerOrders", "MedecinTraitant");
            DropColumn("dbo.CustomerOrders", "Remarque");
        }
    }
}
