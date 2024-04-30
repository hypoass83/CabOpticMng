namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_table_Journal : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Journals",
                c => new
                    {
                        JournalID = c.Int(nullable: false, identity: true),
                        JournalCode = c.String(maxLength: 30),
                        JournalLabel = c.String(),
                        JournalDescription = c.String(),
                    })
                .PrimaryKey(t => t.JournalID)
                .Index(t => t.JournalCode, unique: true);
            
            AddColumn("dbo.Operations", "JournalID", c => c.Int());
            CreateIndex("dbo.Operations", "JournalID");
            AddForeignKey("dbo.Operations", "JournalID", "dbo.Journals", "JournalID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Operations", "JournalID", "dbo.Journals");
            DropIndex("dbo.Journals", new[] { "JournalCode" });
            DropIndex("dbo.Operations", new[] { "JournalID" });
            DropColumn("dbo.Operations", "JournalID");
            DropTable("dbo.Journals");
        }
    }
}
