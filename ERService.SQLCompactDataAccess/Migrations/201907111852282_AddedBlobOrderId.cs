namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBlobOrderId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Blob", "Order_Id", "dbo.Order");
            DropIndex("dbo.Blob", new[] { "Order_Id" });
            RenameColumn(table: "dbo.Blob", name: "Order_Id", newName: "OrderId");
            AlterColumn("dbo.Blob", "OrderId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Blob", "OrderId");
            AddForeignKey("dbo.Blob", "OrderId", "dbo.Order", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Blob", "OrderId", "dbo.Order");
            DropIndex("dbo.Blob", new[] { "OrderId" });
            AlterColumn("dbo.Blob", "OrderId", c => c.Guid());
            RenameColumn(table: "dbo.Blob", name: "OrderId", newName: "Order_Id");
            CreateIndex("dbo.Blob", "Order_Id");
            AddForeignKey("dbo.Blob", "Order_Id", "dbo.Order", "Id");
        }
    }
}
