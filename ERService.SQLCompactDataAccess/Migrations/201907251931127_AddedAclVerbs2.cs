namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAclVerbs2 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Acl", "AclVerbId");
            AddForeignKey("dbo.Acl", "AclVerbId", "dbo.AclVerb", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Acl", "AclVerbId", "dbo.AclVerb");
            DropIndex("dbo.Acl", new[] { "AclVerbId" });
        }
    }
}
