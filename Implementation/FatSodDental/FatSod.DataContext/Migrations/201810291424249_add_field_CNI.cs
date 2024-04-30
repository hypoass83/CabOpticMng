namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_CNI : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AuthoriseSales", "CNI", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AuthoriseSales", "CNI");
        }
    }
}
