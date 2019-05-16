namespace ERService.MSSQLDataAccess.Migrations
{
    using ERService.Business;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ERService.MSSQLDataAccess.ERServiceDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ERService.MSSQLDataAccess.ERServiceDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            //context.Customers.AddOrUpdate(e => e.Id,
            //    new Customer() { Id = new Guid(), FirstName = "Jan", LastName = "Nowak" },
            //    new Customer() { Id = new Guid(), FirstName = "Marek", LastName = "Kawa�ek" },
            //    new Customer() { Id = new Guid(), FirstName = "Anna", LastName = "Nowak" });            

            //context.SaveChanges();
        }

    }
}