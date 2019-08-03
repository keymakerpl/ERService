namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPasswordSalt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "Salt", c => c.String(defaultValue: ""));
            AlterColumn("dbo.User", "Password", p => p.String(defaultValue: ""));
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "Salt");
        }
    }
}
