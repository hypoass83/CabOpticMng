namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Table_ProductDamageLine_ProductDamageReason : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductDamageLines", "ProductDamageReason", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductDamageLines", "ProductDamageReason");
        }
    }
}
