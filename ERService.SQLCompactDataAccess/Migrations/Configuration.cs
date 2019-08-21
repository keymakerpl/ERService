namespace ERService.MSSQLDataAccess.Migrations
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

        protected override void Seed(ERServiceDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            context.AclVerbs.AddOrUpdate(a => a.Name,
                new AclVerb() { Id = new Guid("{BE0F1FBB-960D-40C6-ACA9-63E3E8A7C118}"), Name = "Pe³en dostêp", DefaultValue = 0 },

                new AclVerb() { Id = new Guid("{60FF3DB2-DBB4-4B4D-90D0-DFCE413E1935}"), Name = "Dostêp do konfiguracji aplikacji", DefaultValue = 0 },
                new AclVerb() { Id = new Guid("{D060FFBD-4188-443E-862D-8A637CC395D3}"), Name = "Dostêp do konfiguracji wydruków", DefaultValue = 0 },
                new AclVerb() { Id = new Guid("{C463591E-C323-4DC8-8A2D-F79EEF7D1624}"), Name = "Dostêp do konfiguracji numeracji", DefaultValue = 0 },
                new AclVerb() { Id = new Guid("{C463591E-C323-4DC8-8A2D-F79EEF7D1624}"), Name = "Zarz¹dzanie u¿ytkownikami", DefaultValue = 0 },

                new AclVerb() { Id = new Guid("{7B129DC6-B78B-4521-AAC9-1282912279E9}"), Name = "Dodawanie nowych napraw", DefaultValue = 0 },
                new AclVerb() { Id = new Guid("{C463591E-C323-4DC8-8A2D-F79EEF7D1624}"), Name = "Usuwanie napraw", DefaultValue = 0 },
                new AclVerb() { Id = new Guid("{C463591E-C323-4DC8-8A2D-F79EEF7D1624}"), Name = "Edytowanie napraw", DefaultValue = 0 },

                new AclVerb() { Id = new Guid("{C463591E-C323-4DC8-8A2D-F79EEF7D1624}"), Name = "Dodawanie nowych klientów", DefaultValue = 0 },
                new AclVerb() { Id = new Guid("{C463591E-C323-4DC8-8A2D-F79EEF7D1624}"), Name = "Usuwanie klientów", DefaultValue = 0 },
                new AclVerb() { Id = new Guid("{C463591E-C323-4DC8-8A2D-F79EEF7D1624}"), Name = "Edytowanie klientów", DefaultValue = 0 }
                );

            context.SaveChanges(); // Zapisz verby

            context.Roles.AddOrUpdate(n => n.Name,
                new Role() { Name = "Administrator", IsSystem = true
                });

            context.SaveChanges(); // Przypisz Acle do roli admina

            foreach (var verb in context.AclVerbs)
            {
                context.ACLs.AddOrUpdate(a => a.AclVerbId,
                    new Acl() { AclVerbId = verb.Id, Value = 1, RoleId = context.Roles.Single(r => r.Name == "Administrator").Id });
            }

            context.SaveChanges(); // Przypisz Verby do Acli

            context.Users.AddOrUpdate(u => u.Login,
                new User() { Login = "administrator", IsActive = true, IsAdmin = true, IsSystem = true, 
                    RoleId = context.Roles.Single(r => r.Name == "Administrator").Id,
                    PasswordHash = "/HMO54rRxNa+SBxAH3Mamqn2gbiaydN80pO9BNyPxcB5LMCPTobg6fR9rTTLgo8w9lV4IFdnR0QyKUTfMgFdRvTxQMGIK0zOKXdDT3uQg86Qa7DPAMkiAYv/ipg+9mUbuGwhvSCTEAfA8yQ4JXKiNo6acqWKlSsHN9Ezh48dwX1D4GupU4DsSRigeGZ0eIMoLuH0ofPwCMWeLo/tzaJirGwzeBHvECqWeLjhLaBKQaXPvvrMxzAOaaYSFmFmiSmJoM4hxaj0Y9Sg/vyritqkmN6cjvcPFj71bJTk79Jh8t7rFSR4qUNzqfKC6t6X3lHL2Xh3VAarhxJ+h5P5AsbYMQ==",
                    Salt = "ScbGvPwGi2xjUD5wfe0/Ty3Rot3e6G4NRpXbrIpJ8tf4U6H+dQe414sbeJey3NPifszbOpI0BxSa1O/npi32AQ=="
                }
                );

            context.SaveChanges(); // Dodaj usera i przypisz rolê
        }

    }
}
