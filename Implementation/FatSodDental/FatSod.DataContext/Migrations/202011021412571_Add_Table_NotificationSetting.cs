namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Table_NotificationSetting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotificationSettings",
                c => new
                    {
                        NotificationSettingId = c.Int(nullable: false, identity: true),
                        FrenchMessage = c.String(nullable: false),
                        EnglishMessage = c.String(nullable: false),
                        NotificationType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.NotificationSettingId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.NotificationSettings");
        }
    }
}
