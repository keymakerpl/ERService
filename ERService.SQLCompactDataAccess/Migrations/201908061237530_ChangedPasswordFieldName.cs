namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedPasswordFieldName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "PasswordHash", c => c.String());
            DropColumn("dbo.User", "Password");
        }
        
        public override void Down()
        {
            AddColumn("dbo.User", "Password", c => c.String());
            DropColumn("dbo.User", "PasswordHash");
        }
    }
}
