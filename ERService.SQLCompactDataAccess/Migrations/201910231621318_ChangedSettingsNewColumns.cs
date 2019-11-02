namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedSettingsNewColumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Setting", "ValueType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Setting", "ValueType");
        }
    }
}
