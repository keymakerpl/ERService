namespace ERService.MSSQLDataAccess.Migrations
{
    using CommonServiceLocator;
    using ERService.Business;
    using ERService.Infrastructure.Base.Common;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ERServiceDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            
            if (_config.DatabaseProvider == DatabaseProviders.MySQLServer)
            {
                SetSqlGenerator("MySql.Data.MySqlClient", new ERSMySqlMigrationSqlGenerator());
                AutomaticMigrationDataLossAllowed = true;
            }
        }

        private static IConfig _config
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance(typeof(IConfig)) as IConfig;
                }
                catch (System.Exception)
                {
#if DEBUG
                    return new Config();
#endif
                }
            }
        }

        protected override void Seed(ERServiceDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

#if DEBUG
            context.Database.Log = Console.Write;
#endif

            context.AclVerbs.AddOrUpdate(a => a.Name,
                new AclVerb() { Name = "Dost�p do konfiguracji aplikacji", DefaultValue = 0 },
                new AclVerb() { Name = "Dost�p do konfiguracji wydruk�w", DefaultValue = 0 },
                new AclVerb() { Name = "Dost�p do konfiguracji numeracji", DefaultValue = 0 },
                new AclVerb() { Name = "Zarz�dzanie u�ytkownikami", DefaultValue = 0 },

                new AclVerb() { Name = "Dodawanie nowych napraw", DefaultValue = 0 },
                new AclVerb() { Name = "Usuwanie napraw", DefaultValue = 0 },
                new AclVerb() { Name = "Edytowanie napraw", DefaultValue = 0 },

                new AclVerb() { Name = "Dodawanie nowych klient�w", DefaultValue = 0 },
                new AclVerb() { Name = "Usuwanie klient�w", DefaultValue = 0 },
                new AclVerb() { Name = "Edytowanie klient�w", DefaultValue = 0 }
                );

            context.SaveChanges(); // Zapisz verby

            context.Roles.AddOrUpdate(n => n.Name,
                new Role()
                {
                    Name = "Administrator",
                    IsSystem = true
                });

            context.SaveChanges(); // Przypisz Acle do roli admina

            var roleId = context.Roles.FirstOrDefault(r => r.Name == "Administrator").Id;
            var acls = context.ACLs.Where(a => a.RoleId == roleId);
            foreach (var acl in acls)
            {
                context.ACLs.Remove(acl);
            }
            context.SaveChanges();

            foreach (var verb in context.AclVerbs)
            {
                context.ACLs.Add(
                    new Acl() { AclVerbId = verb.Id, Value = 1, RoleId = roleId });
            }

            context.SaveChanges(); // Przypisz Verby do Acli

            context.Users.AddOrUpdate(u => u.Login,
                new User()
                {
                    Login = "administrator",
                    IsActive = true,
                    IsAdmin = true,
                    IsSystem = true,
                    RoleId = context.Roles.Single(r => r.Name == "Administrator").Id,
                    PasswordHash = "/HMO54rRxNa+SBxAH3Mamqn2gbiaydN80pO9BNyPxcB5LMCPTobg6fR9rTTLgo8w9lV4IFdnR0QyKUTfMgFdRvTxQMGIK0zOKXdDT3uQg86Qa7DPAMkiAYv/ipg+9mUbuGwhvSCTEAfA8yQ4JXKiNo6acqWKlSsHN9Ezh48dwX1D4GupU4DsSRigeGZ0eIMoLuH0ofPwCMWeLo/tzaJirGwzeBHvECqWeLjhLaBKQaXPvvrMxzAOaaYSFmFmiSmJoM4hxaj0Y9Sg/vyritqkmN6cjvcPFj71bJTk79Jh8t7rFSR4qUNzqfKC6t6X3lHL2Xh3VAarhxJ+h5P5AsbYMQ==",
                    Salt = "ScbGvPwGi2xjUD5wfe0/Ty3Rot3e6G4NRpXbrIpJ8tf4U6H+dQe414sbeJey3NPifszbOpI0BxSa1O/npi32AQ=="
                }
                );

            context.SaveChanges(); // Dodaj usera i przypisz rol�

            context.Settings.AddOrUpdate(s => s.Key,
                new Setting()
                {
                    Key = "CompanyName",
                    Category = "CompanyInfo",
                    Value = "Test",
                    ValueType = typeof(string).FullName,
                    Description = "Nazwa firmy"
                },
                new Setting()
                {
                    Key = "CompanyStreet",
                    Category = "CompanyInfo",
                    Value = "",
                    ValueType = typeof(string).FullName,
                    Description = "Ulica przy jakiej prowadzona jest dzia�alno��"
                },
                new Setting()
                {
                    Key = "CompanyNumber",
                    Category = "CompanyInfo",
                    Value = "",
                    ValueType = typeof(string).FullName,
                    Description = "Numer budynku"
                },
                new Setting()
                {
                    Key = "CompanyCity",
                    Category = "CompanyInfo",
                    Value = "",
                    ValueType = typeof(string).FullName,
                    Description = "Miasto prowadzenia dzia�alno�ci"
                },
                new Setting()
                {
                    Key = "CompanyPostCode",
                    Category = "CompanyInfo",
                    Value = "",
                    ValueType = typeof(string).FullName,
                    Description = "Kod pocztowy"
                },
                new Setting()
                {
                    Key = "CompanyNIP",
                    Category = "CompanyInfo",
                    Value = "",
                    ValueType = typeof(string).FullName,
                    Description = "NIP"
                }
                );

            context.HardwareTypes.AddOrUpdate(ht => ht.Name,
                new HardwareType() { Name = "Laptop" },
                new HardwareType() { Name = "Komputer PC" },
                new HardwareType() { Name = "Telefon" },
                new HardwareType() { Name = "Drukarka" },
                new HardwareType() { Name = "Konsola" },
                new HardwareType() { Name = "Nawigacja" },
                new HardwareType() { Name = "Aparat" },
                new HardwareType() { Name = "Monitor" },
                new HardwareType() { Name = "Telewizor" }
                );

            context.OrderStatuses.AddOrUpdate(os => os.Name,
                new OrderStatus() { Name = "Nowa naprawa" },
                new OrderStatus() { Name = "W trakcie naprawy" },
                new OrderStatus() { Name = "Oczekuje na cz�ci" },
                new OrderStatus() { Name = "Naprawa zako�czona" },
                new OrderStatus() { Name = "Naprawa niewykonana" }
                );

            context.OrderTypes.AddOrUpdate(ot => ot.Name,
                new OrderType() { Name = "Gwarancyjna" },
                new OrderType() { Name = "Niegwarancyjna" },
                new OrderType() { Name = "Pogwarancyjna" }
                );

            context.SaveChanges();
        }
    }
}