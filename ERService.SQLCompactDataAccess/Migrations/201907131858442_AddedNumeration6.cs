namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNumeration6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Numeration",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Pattern = c.String(defaultValue: "[MM][RRRRR]"),
                        NextNumber = c.Int(nullable: false, defaultValue: 1),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Numeration");
        }
    }
}
