using ERService.Business;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace ERService.MSSQLDataAccess
{
    public class ERSCreateDatabaseIfNotExists : CreateDatabaseIfNotExists<ERServiceDbContext>
    {
        protected override void Seed(ERServiceDbContext context)
        {
#if DEBUG
            context.Database.Log = Console.Write;
#endif

            context.AclVerbs.AddOrUpdate(a => a.Name,
                new AclVerb() { Name = "Dostęp do konfiguracji aplikacji", DefaultValue = 0 },
                new AclVerb() { Name = "Dostęp do konfiguracji wydruków", DefaultValue = 0 },
                new AclVerb() { Name = "Dostęp do konfiguracji numeracji", DefaultValue = 0 },
                new AclVerb() { Name = "Zarządzanie użytkownikami", DefaultValue = 0 },

                new AclVerb() { Name = "Dodawanie nowych napraw", DefaultValue = 0 },
                new AclVerb() { Name = "Usuwanie napraw", DefaultValue = 0 },
                new AclVerb() { Name = "Edytowanie napraw", DefaultValue = 0 },

                new AclVerb() { Name = "Dodawanie nowych klientów", DefaultValue = 0 },
                new AclVerb() { Name = "Usuwanie klientów", DefaultValue = 0 },
                new AclVerb() { Name = "Edytowanie klientów", DefaultValue = 0 }
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

            context.SaveChanges(); // Dodaj usera i przypisz rolę

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
                    Description = "Ulica przy jakiej prowadzona jest działalność"
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
                    Description = "Miasto prowadzenia działalności"
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
                new OrderStatus() { Name = "Oczekuje na części" },
                new OrderStatus() { Name = "Naprawa zakończona" },
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
