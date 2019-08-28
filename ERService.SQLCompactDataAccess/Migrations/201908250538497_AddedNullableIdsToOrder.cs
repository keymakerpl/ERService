namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNullableIdsToOrder : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Order", "OrderStatusId", "dbo.OrderStatus");
            DropForeignKey("dbo.Order", "OrderTypeId", "dbo.OrderType");
            DropIndex("dbo.Order", new[] { "OrderStatusId" });
            DropIndex("dbo.Order", new[] { "OrderTypeId" });
            AlterColumn("dbo.Order", "OrderStatusId", c => c.Guid());
            AlterColumn("dbo.Order", "OrderTypeId", c => c.Guid());
            CreateIndex("dbo.Order", "OrderStatusId");
            CreateIndex("dbo.Order", "OrderTypeId");
            AddForeignKey("dbo.Order", "OrderStatusId", "dbo.OrderStatus", "Id");
            AddForeignKey("dbo.Order", "OrderTypeId", "dbo.OrderType", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Order", "OrderTypeId", "dbo.OrderType");
            DropForeignKey("dbo.Order", "OrderStatusId", "dbo.OrderStatus");
            DropIndex("dbo.Order", new[] { "OrderTypeId" });
            DropIndex("dbo.Order", new[] { "OrderStatusId" });
            AlterColumn("dbo.Order", "OrderTypeId", c => c.Guid(nullable: false));
            AlterColumn("dbo.Order", "OrderStatusId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Order", "OrderTypeId");
            CreateIndex("dbo.Order", "OrderStatusId");
            AddForeignKey("dbo.Order", "OrderTypeId", "dbo.OrderType", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Order", "OrderStatusId", "dbo.OrderStatus", "Id", cascadeDelete: true);
        }
    }
}
