namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedSettingsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Setting", "Category", c => c.String());
            AddColumn("dbo.Setting", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Setting", "Description");
            DropColumn("dbo.Setting", "Category");
        }
    }
}
