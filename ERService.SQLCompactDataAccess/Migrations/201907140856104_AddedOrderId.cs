namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOrderId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Order", "OrderId", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.Numeration", "NextNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Numeration", "NextNumber", c => c.Int(nullable: false));
            DropColumn("dbo.Order", "OrderId");
        }
    }
}
