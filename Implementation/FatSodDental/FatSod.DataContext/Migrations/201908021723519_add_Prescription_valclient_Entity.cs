namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_Prescription_valclient_Entity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "isPrescritionValidate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "isPrescritionValidate");
        }
    }
}
