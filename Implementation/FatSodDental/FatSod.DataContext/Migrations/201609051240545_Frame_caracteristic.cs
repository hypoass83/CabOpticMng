namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Frame_caracteristic : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GenericProducts", "marque", c => c.String());
            AddColumn("dbo.GenericProducts", "reference", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GenericProducts", "reference");
            DropColumn("dbo.GenericProducts", "marque");
        }
    }
}
