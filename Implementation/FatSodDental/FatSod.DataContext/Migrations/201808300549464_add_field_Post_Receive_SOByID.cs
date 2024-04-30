namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_Post_Receive_SOByID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "PostSOByID", c => c.Int());
            AddColumn("dbo.Sales", "ReceiveSOByID", c => c.Int());
            CreateIndex("dbo.Sales", "PostSOByID");
            CreateIndex("dbo.Sales", "ReceiveSOByID");
            AddForeignKey("dbo.Sales", "PostSOByID", "dbo.Users", "GlobalPersonID");
            AddForeignKey("dbo.Sales", "ReceiveSOByID", "dbo.Users", "GlobalPersonID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sales", "ReceiveSOByID", "dbo.Users");
            DropForeignKey("dbo.Sales", "PostSOByID", "dbo.Users");
            DropIndex("dbo.Sales", new[] { "ReceiveSOByID" });
            DropIndex("dbo.Sales", new[] { "PostSOByID" });
            DropColumn("dbo.Sales", "ReceiveSOByID");
            DropColumn("dbo.Sales", "PostSOByID");
        }
    }
}
