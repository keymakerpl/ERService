namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAclVerbs1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Acl", "AclVerbId", "dbo.AclVerb");
            DropIndex("dbo.Acl", new[] { "AclVerbId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Acl", "AclVerbId");
            AddForeignKey("dbo.Acl", "AclVerbId", "dbo.AclVerb", "Id", cascadeDelete: true);
        }
    }
}
