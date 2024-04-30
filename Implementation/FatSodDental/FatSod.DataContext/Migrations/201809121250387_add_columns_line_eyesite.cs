namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_columns_line_eyesite : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrderLines", "Addition", c => c.String());
            AddColumn("dbo.CustomerOrderLines", "Axis", c => c.String());
            AddColumn("dbo.CustomerOrderLines", "LensNumberCylindricalValue", c => c.String());
            AddColumn("dbo.CustomerOrderLines", "LensNumberSphericalValue", c => c.String());
            AddColumn("dbo.SaleLines", "Addition", c => c.String());
            AddColumn("dbo.SaleLines", "Axis", c => c.String());
            AddColumn("dbo.SaleLines", "LensNumberCylindricalValue", c => c.String());
            AddColumn("dbo.SaleLines", "LensNumberSphericalValue", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SaleLines", "LensNumberSphericalValue");
            DropColumn("dbo.SaleLines", "LensNumberCylindricalValue");
            DropColumn("dbo.SaleLines", "Axis");
            DropColumn("dbo.SaleLines", "Addition");
            DropColumn("dbo.CustomerOrderLines", "LensNumberSphericalValue");
            DropColumn("dbo.CustomerOrderLines", "LensNumberCylindricalValue");
            DropColumn("dbo.CustomerOrderLines", "Axis");
            DropColumn("dbo.CustomerOrderLines", "Addition");
        }
    }
}
