namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedSettingsToSetting : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Settings", newName: "Setting");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Setting", newName: "Settings");
        }
    }
}
