namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedGuidIdGenerated : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CustomerAddress", "CustomerId", "dbo.Customer");
            DropPrimaryKey("dbo.Customer");
            AlterColumn("dbo.Customer", "Id", c => c.Guid(nullable: false, identity: true, defaultValueSql: "NEWID()"));
            AddPrimaryKey("dbo.Customer", "Id");
            AddForeignKey("dbo.CustomerAddress", "CustomerId", "dbo.Customer", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerAddress", "CustomerId", "dbo.Customer");
            DropPrimaryKey("dbo.Customer");
            AlterColumn("dbo.Customer", "Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.Customer", "Id");
            AddForeignKey("dbo.CustomerAddress", "CustomerId", "dbo.Customer", "Id", cascadeDelete: true);
        }
    }
}
