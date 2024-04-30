namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modif_customernum_Type : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Customers", "CustomerNumber", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Customers", "CustomerNumber", c => c.Int(nullable: false));
        }
    }
}
