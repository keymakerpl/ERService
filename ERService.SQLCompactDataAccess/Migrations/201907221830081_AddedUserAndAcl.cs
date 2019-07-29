namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserAndAcl : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Acl",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Value = c.Int(nullable: false),
                        DefaultValue = c.Int(nullable: false),
                        Description = c.String(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Role_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Role", t => t.Role_Id)
                .Index(t => t.Role_Id);
            
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Login = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false),
                        FirstName = c.String(maxLength: 50),
                        LastName = c.String(maxLength: 50),
                        PhoneNumber = c.String(maxLength: 50),
                        IsActive = c.Int(nullable: false),
                        IsAdmin = c.Int(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Role_Id = c.Guid(),
                        RoleId_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Role", t => t.Role_Id)
                .ForeignKey("dbo.Role", t => t.RoleId_Id)
                .Index(t => t.Role_Id)
                .Index(t => t.RoleId_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.User", "RoleId_Id", "dbo.Role");
            DropForeignKey("dbo.User", "Role_Id", "dbo.Role");
            DropForeignKey("dbo.Acl", "Role_Id", "dbo.Role");
            DropIndex("dbo.User", new[] { "RoleId_Id" });
            DropIndex("dbo.User", new[] { "Role_Id" });
            DropIndex("dbo.Acl", new[] { "Role_Id" });
            DropTable("dbo.User");
            DropTable("dbo.Role");
            DropTable("dbo.Acl");
        }
    }
}
