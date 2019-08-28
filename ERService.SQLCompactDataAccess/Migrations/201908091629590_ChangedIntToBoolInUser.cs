namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedIntToBoolInUser : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.User", "IsActive", c => c.Boolean(nullable: false));
            AlterColumn("dbo.User", "IsAdmin", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.User", "IsAdmin", c => c.Int(nullable: false));
            AlterColumn("dbo.User", "IsActive", c => c.Int(nullable: false));
        }
    }
}
