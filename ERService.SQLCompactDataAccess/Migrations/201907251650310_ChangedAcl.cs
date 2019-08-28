namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedAcl : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Acl", "Value");
            DropColumn("dbo.Acl", "DefaultValue");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Acl", "DefaultValue", c => c.Int(nullable: false));
            AddColumn("dbo.Acl", "Value", c => c.Int(nullable: false));
        }
    }
}
