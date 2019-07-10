namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOrderStatusTypeId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Order", "OrderStatus_Id", "dbo.OrderStatus");
            DropForeignKey("dbo.Order", "OrderType_Id", "dbo.OrderType");
            DropIndex("dbo.Order", new[] { "OrderStatus_Id" });
            DropIndex("dbo.Order", new[] { "OrderType_Id" });
            RenameColumn(table: "dbo.Order", name: "OrderStatus_Id", newName: "OrderStatusId");
            RenameColumn(table: "dbo.Order", name: "OrderType_Id", newName: "OrderTypeId");
            AlterColumn("dbo.Order", "OrderStatusId", c => c.Guid(nullable: false));
            AlterColumn("dbo.Order", "OrderTypeId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Order", "OrderStatusId");
            CreateIndex("dbo.Order", "OrderTypeId");
            AddForeignKey("dbo.Order", "OrderStatusId", "dbo.OrderStatus", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Order", "OrderTypeId", "dbo.OrderType", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Order", "OrderTypeId", "dbo.OrderType");
            DropForeignKey("dbo.Order", "OrderStatusId", "dbo.OrderStatus");
            DropIndex("dbo.Order", new[] { "OrderTypeId" });
            DropIndex("dbo.Order", new[] { "OrderStatusId" });
            AlterColumn("dbo.Order", "OrderTypeId", c => c.Guid());
            AlterColumn("dbo.Order", "OrderStatusId", c => c.Guid());
            RenameColumn(table: "dbo.Order", name: "OrderTypeId", newName: "OrderType_Id");
            RenameColumn(table: "dbo.Order", name: "OrderStatusId", newName: "OrderStatus_Id");
            CreateIndex("dbo.Order", "OrderType_Id");
            CreateIndex("dbo.Order", "OrderStatus_Id");
            AddForeignKey("dbo.Order", "OrderType_Id", "dbo.OrderType", "Id");
            AddForeignKey("dbo.Order", "OrderStatus_Id", "dbo.OrderStatus", "Id");
        }
    }
}
