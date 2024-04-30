namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class customerOrder_add_Mnt_assureur : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrders", "VerreAssurance", c => c.Double(nullable: false));
            AddColumn("dbo.CustomerOrders", "MontureAssurance", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerOrders", "MontureAssurance");
            DropColumn("dbo.CustomerOrders", "VerreAssurance");
        }
    }
}
