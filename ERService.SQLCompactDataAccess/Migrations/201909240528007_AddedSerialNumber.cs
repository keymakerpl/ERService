namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSerialNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Hardware", "SerialNumber", c => c.String(maxLength: 80));
            AlterColumn("dbo.Hardware", "Name", c => c.String(maxLength: 80));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Hardware", "Name", c => c.String(maxLength: 50));
            DropColumn("dbo.Hardware", "SerialNumber");
        }
    }
}
