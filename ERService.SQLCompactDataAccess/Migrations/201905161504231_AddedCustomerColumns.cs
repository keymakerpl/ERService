namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCustomerColumns : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.CustomerAddress");
            AddColumn("dbo.Customer", "CompanyName", c => c.String(maxLength: 50));
            AddColumn("dbo.Customer", "NIP", c => c.String(maxLength: 50));
            AddColumn("dbo.Customer", "Email2", c => c.String(maxLength: 50));
            AddColumn("dbo.Customer", "PhoneNumber", c => c.String(maxLength: 20));
            AddColumn("dbo.Customer", "PhoneNumber2", c => c.String(maxLength: 20));
            AddColumn("dbo.Customer", "Description", c => c.String(maxLength: 50));
            AlterColumn("dbo.CustomerAddress", "Id", c => c.Guid(nullable: false, identity: true));
            AddPrimaryKey("dbo.CustomerAddress", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.CustomerAddress");
            AlterColumn("dbo.CustomerAddress", "Id", c => c.Guid(nullable: false));
            DropColumn("dbo.Customer", "Description");
            DropColumn("dbo.Customer", "PhoneNumber2");
            DropColumn("dbo.Customer", "PhoneNumber");
            DropColumn("dbo.Customer", "Email2");
            DropColumn("dbo.Customer", "NIP");
            DropColumn("dbo.Customer", "CompanyName");
            AddPrimaryKey("dbo.CustomerAddress", "Id");
        }
    }
}
