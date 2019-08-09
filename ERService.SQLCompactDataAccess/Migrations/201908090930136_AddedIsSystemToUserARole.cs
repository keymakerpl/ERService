namespace ERService.MSSQLDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsSystemToUserARole : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Role", "IsSystem", c => c.Boolean(nullable: false));
            AddColumn("dbo.User", "IsSystem", c => c.Boolean(nullable: false));
            CreateIndex("dbo.User", "Login", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.User", new[] { "Login" });
            DropColumn("dbo.User", "IsSystem");
            DropColumn("dbo.Role", "IsSystem");
        }
    }
}
