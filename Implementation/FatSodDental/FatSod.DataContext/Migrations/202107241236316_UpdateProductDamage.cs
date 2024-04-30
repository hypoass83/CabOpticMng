namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProductDamage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductDamages", "LensMountingDamageBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductDamages", "LensMountingDamageBy");
        }
    }
}
