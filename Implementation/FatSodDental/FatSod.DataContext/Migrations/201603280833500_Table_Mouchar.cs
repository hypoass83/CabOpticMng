namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Table_Mouchar : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Mouchars",
                c => new
                    {
                        MoucharID = c.Int(nullable: false, identity: true),
                        MoucharDate = c.DateTime(nullable: false),
                        SneackHour = c.String(maxLength: 6),
                        MoucharUserID = c.Int(nullable: false),
                        MoucharAction = c.String(),
                        MoucharDescription = c.String(),
                        MoucharOperationType = c.String(),
                        MoucharProcedureName = c.String(),
                        MoucharHost = c.String(),
                        MoucharHostAdress = c.String(),
                        MoucharBranchID = c.Int(nullable: false),
                        MoucharBusinessDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MoucharID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Mouchars");
        }
    }
}
