using System.Collections.Generic;

namespace ERService.SQLLiteDataAccess
{
    using ERService.Business;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ERServiceDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        /// <summary>
        /// Wrzucamy do db przykładowe dane
        /// </summary>
        /// <param name="context"></param>
        protected override void Seed(ERServiceDbContext context)
        {
            context.Customers.AddOrUpdate(e => e.FirstName,
                new Customer() { FirstName = "Jan", LastName = "Nowak" },
                new Customer() { FirstName = "Marek", LastName = "Kawałek" },
                new Customer() { FirstName = "Anna", LastName = "Nowak" });

            context.Addresses.AddOrUpdate(pl => pl.City,
                new CustomerAddress() { City = "Zawiercie", Street = "Nowe Zawiercie 3", CustomerId = context.Customers.First().Id });

            context.SaveChanges();

            //context.FriendPhoneNumbers.AddOrUpdate(nm => nm.Number,
            //    new FriendPhoneNumber() { Number = "+48 12345678", FriendId = context.Friends.First().Id });

            //context.Meetings.AddOrUpdate(m => m.Name,
            //    new Meeting()
            //    {
            //        Name = "Programming",
            //        DateFrom = new DateTime(2019, 5, 26),
            //        DateTo = new DateTime(2018, 5, 26),
            //        Friends = new List<Friend>
            //        {
            //            context.Friends.Single(f => f.FirstName == "Radek" && f.LastName == "Kurek")
            //        }
            //    });
        }
    }
}
