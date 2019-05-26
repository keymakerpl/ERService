namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOrdersTypesConfig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Hardware",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                        Name = c.String(maxLength: 50),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Type_Id = c.Guid(),
                        Order_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.HardwareType", t => t.Type_Id)
                .ForeignKey("dbo.Order", t => t.Order_Id)
                .Index(t => t.Type_Id)
                .Index(t => t.Order_Id);
            
            CreateTable(
                "dbo.HardwareType",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                        Name = c.String(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Order",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                        Number = c.String(maxLength: 50),
                        DateAdded = c.DateTime(nullable: false),
                        DateEnded = c.DateTime(nullable: false),
                        Cost = c.String(maxLength: 50),
                        Fault = c.String(maxLength: 1000),
                        Solution = c.String(maxLength: 1000),
                        Comment = c.String(maxLength: 1000),
                        ExternalNumber = c.String(maxLength: 50),
                        Progress = c.Int(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Customer_Id = c.Guid(),
                        OrderStatus_Id = c.Guid(),
                        OrderType_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customer", t => t.Customer_Id)
                .ForeignKey("dbo.OrderStatus", t => t.OrderStatus_Id)
                .ForeignKey("dbo.OrderType", t => t.OrderType_Id)
                .Index(t => t.Customer_Id)
                .Index(t => t.OrderStatus_Id)
                .Index(t => t.OrderType_Id);
            
            CreateTable(
                "dbo.OrderStatus",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                        Name = c.String(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderType",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                        Name = c.String(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                        Key = c.String(),
                        Value = c.String(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Order", "OrderType_Id", "dbo.OrderType");
            DropForeignKey("dbo.Order", "OrderStatus_Id", "dbo.OrderStatus");
            DropForeignKey("dbo.Hardware", "Order_Id", "dbo.Order");
            DropForeignKey("dbo.Order", "Customer_Id", "dbo.Customer");
            DropForeignKey("dbo.Hardware", "Type_Id", "dbo.HardwareType");
            DropIndex("dbo.Order", new[] { "OrderType_Id" });
            DropIndex("dbo.Order", new[] { "OrderStatus_Id" });
            DropIndex("dbo.Order", new[] { "Customer_Id" });
            DropIndex("dbo.Hardware", new[] { "Order_Id" });
            DropIndex("dbo.Hardware", new[] { "Type_Id" });
            DropTable("dbo.Settings");
            DropTable("dbo.OrderType");
            DropTable("dbo.OrderStatus");
            DropTable("dbo.Order");
            DropTable("dbo.HardwareType");
            DropTable("dbo.Hardware");
        }
    }
}
