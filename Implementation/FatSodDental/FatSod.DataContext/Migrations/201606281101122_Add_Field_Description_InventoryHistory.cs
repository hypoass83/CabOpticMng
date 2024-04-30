namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Field_Description_InventoryHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryHistorics", "Description", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InventoryHistorics", "Description");
        }
    }
}
