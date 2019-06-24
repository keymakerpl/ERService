namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCustomItem1 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.HwCustomItem", "CustomItemId");
            AddForeignKey("dbo.HwCustomItem", "CustomItemId", "dbo.CustomItem", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HwCustomItem", "CustomItemId", "dbo.CustomItem");
            DropIndex("dbo.HwCustomItem", new[] { "CustomItemId" });
        }
    }
}
