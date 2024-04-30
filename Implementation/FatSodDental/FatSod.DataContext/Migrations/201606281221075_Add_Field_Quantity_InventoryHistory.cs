namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Field_Quantity_InventoryHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryHistorics", "Quantity", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InventoryHistorics", "Quantity");
        }
    }
}
