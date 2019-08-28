namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNullRoleToUser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.User", "RoleId", "dbo.Role");
            DropIndex("dbo.User", new[] { "RoleId" });
            AlterColumn("dbo.User", "RoleId", c => c.Guid());
            CreateIndex("dbo.User", "RoleId");
            AddForeignKey("dbo.User", "RoleId", "dbo.Role", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.User", "RoleId", "dbo.Role");
            DropIndex("dbo.User", new[] { "RoleId" });
            AlterColumn("dbo.User", "RoleId", c => c.Guid(nullable: false));
            CreateIndex("dbo.User", "RoleId");
            AddForeignKey("dbo.User", "RoleId", "dbo.Role", "Id", cascadeDelete: true);
        }
    }
}
