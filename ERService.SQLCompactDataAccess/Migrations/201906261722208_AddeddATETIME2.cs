namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddeddATETIME2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Order", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Order", "DateEnded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Order", "DateEnded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Order", "DateAdded", c => c.DateTime(nullable: false));
        }
    }
}
