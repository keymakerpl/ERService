namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAclVerbs : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.User", "Role_Id", "dbo.Role");
            DropForeignKey("dbo.User", "RoleId_Id", "dbo.Role");
            DropIndex("dbo.User", new[] { "Role_Id" });
            DropIndex("dbo.User", new[] { "RoleId_Id" });
            CreateTable(
                "dbo.AclVerb",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        DefaultValue = c.Int(nullable: false),
                        Description = c.String(maxLength: 50),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Acl", "AclVerbId", c => c.Guid(nullable: false));
            AddColumn("dbo.Acl", "Value", c => c.Int(nullable: false));
            AddColumn("dbo.User", "RoleId", c => c.Guid(nullable: false));
            AlterColumn("dbo.User", "Password", c => c.String());
            CreateIndex("dbo.Acl", "AclVerbId");
            AddForeignKey("dbo.Acl", "AclVerbId", "dbo.AclVerb", "Id", cascadeDelete: true);
            DropColumn("dbo.Acl", "Name");
            DropColumn("dbo.Acl", "Description");
            DropColumn("dbo.User", "Role_Id");
            DropColumn("dbo.User", "RoleId_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.User", "RoleId_Id", c => c.Guid());
            AddColumn("dbo.User", "Role_Id", c => c.Guid());
            AddColumn("dbo.Acl", "Description", c => c.String());
            AddColumn("dbo.Acl", "Name", c => c.String(nullable: false, maxLength: 50));
            DropForeignKey("dbo.Acl", "AclVerbId", "dbo.AclVerb");
            DropIndex("dbo.Acl", new[] { "AclVerbId" });
            AlterColumn("dbo.User", "Password", c => c.String(nullable: false));
            DropColumn("dbo.User", "RoleId");
            DropColumn("dbo.Acl", "Value");
            DropColumn("dbo.Acl", "AclVerbId");
            DropTable("dbo.AclVerb");
            CreateIndex("dbo.User", "RoleId_Id");
            CreateIndex("dbo.User", "Role_Id");
            AddForeignKey("dbo.User", "RoleId_Id", "dbo.Role", "Id");
            AddForeignKey("dbo.User", "Role_Id", "dbo.Role", "Id");
        }
    }
}
