namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_CNI_sale : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "CNI", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sales", "CNI");
        }
    }
}
