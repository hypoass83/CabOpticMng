namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Fiels_CustomerName_on_Sale : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "CustomerName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sales", "CustomerName");
        }
    }
}
