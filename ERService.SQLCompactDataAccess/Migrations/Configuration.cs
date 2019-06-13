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
            //    new Customer() { Id = new Guid(), FirstName = "Marek", LastName = "Kawa³ek" },
            //    new Customer() { Id = new Guid(), FirstName = "Anna", LastName = "Nowak" });            

            //context.HardwareTypes.AddOrUpdate(n => n.Name ,new HardwareType() { Name = "Laptop" });
            //context.SaveChanges();

            //context.CustomItems.AddOrUpdate(k => k.Key, new CustomItem() { HardwareType = context.HardwareTypes.First(), Key = "Procesor" });
            //context.SaveChanges();

            //context.Hardwares.AddOrUpdate(n => n.Name, new Hardware() { Name = "Samsung"});
            //context.SaveChanges();

            //context.HardwareCustomItems
            //    .AddOrUpdate(v => v.Value, new HwCustomItem() {CustomItemId = context.CustomItems.First().Id, Hardware = context.Hardwares.First(), Value = "Intel Core i5" });
            //context.SaveChanges();


            //context.SaveChanges();
        }

    }
}
