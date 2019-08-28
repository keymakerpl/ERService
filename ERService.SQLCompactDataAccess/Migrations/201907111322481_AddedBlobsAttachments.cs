namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBlobsAttachments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Blob",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        FileName = c.String(nullable: false, maxLength: 300),
                        Description = c.String(),
                        Checksum = c.String(),
                        Data = c.Binary(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Order_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Order", t => t.Order_Id)
                .Index(t => t.Order_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Blob", "Order_Id", "dbo.Order");
            DropIndex("dbo.Blob", new[] { "Order_Id" });
            DropTable("dbo.Blob");
        }
    }
}
