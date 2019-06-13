namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCustomItem : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Hardware", "Type_Id", "dbo.HardwareType");
            DropIndex("dbo.Hardware", new[] { "Type_Id" });
            CreateTable(
                "dbo.CustomItem",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Key = c.String(nullable: false, maxLength: 50),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        HardwareType_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.HardwareType", t => t.HardwareType_Id)
                .Index(t => t.HardwareType_Id);
            
            CreateTable(
                "dbo.HwCustomItem",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CustomItemId = c.Guid(nullable: false),
                        Value = c.String(maxLength: 200),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Hardware_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Hardware", t => t.Hardware_Id)
                .Index(t => t.Hardware_Id);
            
            AddColumn("dbo.CustomerAddress", "HouseNumber", c => c.String());
            AlterColumn("dbo.Customer", "FirstName", c => c.String(maxLength: 50));
            AlterColumn("dbo.Customer", "LastName", c => c.String(nullable: false, maxLength: 50));
            DropColumn("dbo.Hardware", "Type_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Hardware", "Type_Id", c => c.Guid());
            DropForeignKey("dbo.HwCustomItem", "Hardware_Id", "dbo.Hardware");
            DropForeignKey("dbo.CustomItem", "HardwareType_Id", "dbo.HardwareType");
            DropIndex("dbo.HwCustomItem", new[] { "Hardware_Id" });
            DropIndex("dbo.CustomItem", new[] { "HardwareType_Id" });
            AlterColumn("dbo.Customer", "LastName", c => c.String(maxLength: 50));
            AlterColumn("dbo.Customer", "FirstName", c => c.String(nullable: false, maxLength: 50));
            DropColumn("dbo.CustomerAddress", "HouseNumber");
            DropTable("dbo.HwCustomItem");
            DropTable("dbo.CustomItem");
            CreateIndex("dbo.Hardware", "Type_Id");
            AddForeignKey("dbo.Hardware", "Type_Id", "dbo.HardwareType", "Id");
        }
    }
}
