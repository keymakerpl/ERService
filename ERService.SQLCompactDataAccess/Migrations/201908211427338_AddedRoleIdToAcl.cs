namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRoleIdToAcl : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Acl", "Role_Id", "dbo.Role");
            DropIndex("dbo.Acl", new[] { "Role_Id" });
            RenameColumn(table: "dbo.Acl", name: "Role_Id", newName: "RoleId");
            AlterColumn("dbo.Acl", "RoleId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Acl", "RoleId");
            AddForeignKey("dbo.Acl", "RoleId", "dbo.Role", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Acl", "RoleId", "dbo.Role");
            DropIndex("dbo.Acl", new[] { "RoleId" });
            AlterColumn("dbo.Acl", "RoleId", c => c.Guid());
            RenameColumn(table: "dbo.Acl", name: "RoleId", newName: "Role_Id");
            CreateIndex("dbo.Acl", "Role_Id");
            AddForeignKey("dbo.Acl", "Role_Id", "dbo.Role", "Id");
        }
    }
}
