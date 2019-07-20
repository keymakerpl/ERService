namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOrderCusId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Order", "Customer_Id", "dbo.Customer");
            DropIndex("dbo.Order", new[] { "Customer_Id" });
            RenameColumn(table: "dbo.Order", name: "Customer_Id", newName: "CustomerId");
            AlterColumn("dbo.Order", "CustomerId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Order", "CustomerId");
            AddForeignKey("dbo.Order", "CustomerId", "dbo.Customer", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Order", "CustomerId", "dbo.Customer");
            DropIndex("dbo.Order", new[] { "CustomerId" });
            AlterColumn("dbo.Order", "CustomerId", c => c.Guid());
            RenameColumn(table: "dbo.Order", name: "CustomerId", newName: "Customer_Id");
            CreateIndex("dbo.Order", "Customer_Id");
            AddForeignKey("dbo.Order", "Customer_Id", "dbo.Customer", "Id");
        }
    }
}
