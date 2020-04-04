namespace ERService.MSSQLDataAccess.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Acl",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    AclVerbId = c.Guid(nullable: false),
                    RoleId = c.Guid(nullable: false),
                    Value = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AclVerb", t => t.AclVerbId, cascadeDelete: true)
                .ForeignKey("dbo.Role", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.AclVerbId)
                .Index(t => t.RoleId);

            CreateTable(
                "dbo.AclVerb",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    Name = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                    DefaultValue = c.Int(nullable: false),
                    Description = c.String(maxLength: 50, storeType: "nvarchar"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.CustomerAddress",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    CustomerId = c.Guid(nullable: false),
                    Street = c.String(unicode: false),
                    HouseNumber = c.String(unicode: false),
                    City = c.String(unicode: false),
                    Postcode = c.String(unicode: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customer", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);

            CreateTable(
                "dbo.Customer",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    FirstName = c.String(maxLength: 50, storeType: "nvarchar"),
                    LastName = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                    CompanyName = c.String(maxLength: 50, storeType: "nvarchar"),
                    NIP = c.String(maxLength: 50, storeType: "nvarchar"),
                    Email = c.String(maxLength: 50, storeType: "nvarchar"),
                    Email2 = c.String(maxLength: 50, storeType: "nvarchar"),
                    PhoneNumber = c.String(nullable: false, maxLength: 20, storeType: "nvarchar"),
                    PhoneNumber2 = c.String(maxLength: 20, storeType: "nvarchar"),
                    Description = c.String(maxLength: 50, storeType: "nvarchar"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Order",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    OrderId = c.Int(nullable: false, identity: true),
                    CustomerId = c.Guid(),
                    Number = c.String(maxLength: 50, storeType: "nvarchar"),
                    DateAdded = c.DateTime(nullable: false, precision: 0),
                    DateEnded = c.DateTime(nullable: false, precision: 0),
                    OrderStatusId = c.Guid(),
                    OrderTypeId = c.Guid(),
                    Cost = c.String(maxLength: 50, storeType: "nvarchar"),
                    Fault = c.String(maxLength: 1000, storeType: "nvarchar"),
                    Solution = c.String(maxLength: 1000, storeType: "nvarchar"),
                    Comment = c.String(maxLength: 1000, storeType: "nvarchar"),
                    ExternalNumber = c.String(maxLength: 50, storeType: "nvarchar"),
                    Progress = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customer", t => t.CustomerId)
                .ForeignKey("dbo.OrderStatus", t => t.OrderStatusId)
                .ForeignKey("dbo.OrderType", t => t.OrderTypeId)
                .Index(t => t.CustomerId)
                .Index(t => t.OrderStatusId)
                .Index(t => t.OrderTypeId);

            CreateTable(
                "dbo.Blob",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    FileName = c.String(nullable: false, maxLength: 300, storeType: "nvarchar"),
                    Description = c.String(unicode: false),
                    Checksum = c.String(unicode: false),
                    Size = c.Int(nullable: false),
                    OrderId = c.Guid(),
                    Data = c.Binary(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Order", t => t.OrderId)
                .Index(t => t.OrderId);

            CreateTable(
                "dbo.Hardware",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    Name = c.String(maxLength: 80, storeType: "nvarchar"),
                    SerialNumber = c.String(maxLength: 80, storeType: "nvarchar"),
                    Order_Id = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Order", t => t.Order_Id)
                .Index(t => t.Order_Id);

            CreateTable(
                "dbo.HwCustomItem",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    CustomItemId = c.Guid(nullable: false),
                    Value = c.String(maxLength: 200, storeType: "nvarchar"),
                    HardwareId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CustomItem", t => t.CustomItemId, cascadeDelete: true)
                .ForeignKey("dbo.Hardware", t => t.HardwareId, cascadeDelete: true)
                .Index(t => t.CustomItemId)
                .Index(t => t.HardwareId);

            CreateTable(
                "dbo.CustomItem",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    Key = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                    HardwareTypeId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.HardwareType", t => t.HardwareTypeId, cascadeDelete: true)
                .Index(t => t.HardwareTypeId);

            CreateTable(
                "dbo.HardwareType",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    Name = c.String(unicode: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.OrderStatus",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    Name = c.String(unicode: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.OrderType",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    Name = c.String(unicode: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Numeration",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    Name = c.String(maxLength: 50, storeType: "nvarchar"),
                    Pattern = c.String(unicode: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.PrintTemplate",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    Name = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                    Template = c.String(unicode: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Role",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    Name = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                    IsSystem = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.User",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    Login = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                    PasswordHash = c.String(unicode: false),
                    Salt = c.String(unicode: false),
                    FirstName = c.String(maxLength: 50, storeType: "nvarchar"),
                    LastName = c.String(maxLength: 50, storeType: "nvarchar"),
                    PhoneNumber = c.String(maxLength: 50, storeType: "nvarchar"),
                    IsActive = c.Boolean(nullable: false),
                    IsAdmin = c.Boolean(nullable: false),
                    IsSystem = c.Boolean(nullable: false),
                    RoleId = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Role", t => t.RoleId)
                .Index(t => t.Login, unique: true)
                .Index(t => t.RoleId);

            CreateTable(
                "dbo.Setting",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    Key = c.String(unicode: false),
                    Value = c.String(unicode: false),
                    ValueType = c.String(unicode: false),
                    Category = c.String(unicode: false),
                    Description = c.String(unicode: false),
                })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.User", "RoleId", "dbo.Role");
            DropForeignKey("dbo.Acl", "RoleId", "dbo.Role");
            DropForeignKey("dbo.Order", "OrderTypeId", "dbo.OrderType");
            DropForeignKey("dbo.Order", "OrderStatusId", "dbo.OrderStatus");
            DropForeignKey("dbo.Hardware", "Order_Id", "dbo.Order");
            DropForeignKey("dbo.HwCustomItem", "HardwareId", "dbo.Hardware");
            DropForeignKey("dbo.HwCustomItem", "CustomItemId", "dbo.CustomItem");
            DropForeignKey("dbo.CustomItem", "HardwareTypeId", "dbo.HardwareType");
            DropForeignKey("dbo.Order", "CustomerId", "dbo.Customer");
            DropForeignKey("dbo.Blob", "OrderId", "dbo.Order");
            DropForeignKey("dbo.CustomerAddress", "CustomerId", "dbo.Customer");
            DropForeignKey("dbo.Acl", "AclVerbId", "dbo.AclVerb");
            DropIndex("dbo.User", new[] { "RoleId" });
            DropIndex("dbo.User", new[] { "Login" });
            DropIndex("dbo.CustomItem", new[] { "HardwareTypeId" });
            DropIndex("dbo.HwCustomItem", new[] { "HardwareId" });
            DropIndex("dbo.HwCustomItem", new[] { "CustomItemId" });
            DropIndex("dbo.Hardware", new[] { "Order_Id" });
            DropIndex("dbo.Blob", new[] { "OrderId" });
            DropIndex("dbo.Order", new[] { "OrderTypeId" });
            DropIndex("dbo.Order", new[] { "OrderStatusId" });
            DropIndex("dbo.Order", new[] { "CustomerId" });
            DropIndex("dbo.CustomerAddress", new[] { "CustomerId" });
            DropIndex("dbo.Acl", new[] { "RoleId" });
            DropIndex("dbo.Acl", new[] { "AclVerbId" });
            DropTable("dbo.Setting");
            DropTable("dbo.User");
            DropTable("dbo.Role");
            DropTable("dbo.PrintTemplate");
            DropTable("dbo.Numeration");
            DropTable("dbo.OrderType");
            DropTable("dbo.OrderStatus");
            DropTable("dbo.HardwareType");
            DropTable("dbo.CustomItem");
            DropTable("dbo.HwCustomItem");
            DropTable("dbo.Hardware");
            DropTable("dbo.Blob");
            DropTable("dbo.Order");
            DropTable("dbo.Customer");
            DropTable("dbo.CustomerAddress");
            DropTable("dbo.AclVerb");
            DropTable("dbo.Acl");
        }
    }
}
