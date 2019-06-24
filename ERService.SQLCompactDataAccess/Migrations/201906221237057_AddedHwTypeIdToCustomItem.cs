namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedHwTypeIdToCustomItem : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CustomItem", "HardwareType_Id", "dbo.HardwareType");
            DropIndex("dbo.CustomItem", new[] { "HardwareType_Id" });
            RenameColumn(table: "dbo.CustomItem", name: "HardwareType_Id", newName: "HardwareTypeId");
            AlterColumn("dbo.CustomItem", "HardwareTypeId", c => c.Guid(nullable: false));
            CreateIndex("dbo.CustomItem", "HardwareTypeId");
            AddForeignKey("dbo.CustomItem", "HardwareTypeId", "dbo.HardwareType", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomItem", "HardwareTypeId", "dbo.HardwareType");
            DropIndex("dbo.CustomItem", new[] { "HardwareTypeId" });
            AlterColumn("dbo.CustomItem", "HardwareTypeId", c => c.Guid());
            RenameColumn(table: "dbo.CustomItem", name: "HardwareTypeId", newName: "HardwareType_Id");
            CreateIndex("dbo.CustomItem", "HardwareType_Id");
            AddForeignKey("dbo.CustomItem", "HardwareType_Id", "dbo.HardwareType", "Id");
        }
    }
}
