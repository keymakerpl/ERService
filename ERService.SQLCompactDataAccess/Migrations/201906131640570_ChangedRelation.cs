namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedRelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.HwCustomItem", "Hardware_Id", "dbo.Hardware");
            DropIndex("dbo.HwCustomItem", new[] { "Hardware_Id" });
            RenameColumn(table: "dbo.HwCustomItem", name: "Hardware_Id", newName: "HardwareId");
            AlterColumn("dbo.HwCustomItem", "HardwareId", c => c.Guid(nullable: false));
            CreateIndex("dbo.HwCustomItem", "HardwareId");
            AddForeignKey("dbo.HwCustomItem", "HardwareId", "dbo.Hardware", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HwCustomItem", "HardwareId", "dbo.Hardware");
            DropIndex("dbo.HwCustomItem", new[] { "HardwareId" });
            AlterColumn("dbo.HwCustomItem", "HardwareId", c => c.Guid());
            RenameColumn(table: "dbo.HwCustomItem", name: "HardwareId", newName: "Hardware_Id");
            CreateIndex("dbo.HwCustomItem", "Hardware_Id");
            AddForeignKey("dbo.HwCustomItem", "Hardware_Id", "dbo.Hardware", "Id");
        }
    }
}
