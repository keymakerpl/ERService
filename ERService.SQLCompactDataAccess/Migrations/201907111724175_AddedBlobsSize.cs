namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBlobsSize : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Blob", "Size", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Blob", "Size");
        }
    }
}
