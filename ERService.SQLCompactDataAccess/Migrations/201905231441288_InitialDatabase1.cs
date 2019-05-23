namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDatabase1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Customer", "PhoneNumber", c => c.String(nullable: false, maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Customer", "PhoneNumber", c => c.String(maxLength: 20));
        }
    }
}
