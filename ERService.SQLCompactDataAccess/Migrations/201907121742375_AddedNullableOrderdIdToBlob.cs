namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNullableOrderdIdToBlob : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Blob", "OrderId", "dbo.Order");
            DropIndex("dbo.Blob", new[] { "OrderId" });
            AlterColumn("dbo.Blob", "OrderId", c => c.Guid());
            CreateIndex("dbo.Blob", "OrderId");
            AddForeignKey("dbo.Blob", "OrderId", "dbo.Order", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Blob", "OrderId", "dbo.Order");
            DropIndex("dbo.Blob", new[] { "OrderId" });
            AlterColumn("dbo.Blob", "OrderId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Blob", "OrderId");
            AddForeignKey("dbo.Blob", "OrderId", "dbo.Order", "Id", cascadeDelete: true);
        }
    }
}
