namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedDescriptionMaxLengInCustomer : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Customer", "Description", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Customer", "Description", c => c.String(maxLength: 50));
        }
    }
}
