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
                new AclVerb() { Id = new Guid("{43E29522-E318-4666-B3CD-C4760354DA94}"), Name = "Zarz¹dzanie u¿ytkownikami", DefaultValue = 0 },
                new AclVerb() { Id = new Guid("{EB6607C6-8D6B-4702-851C-21605D5F3341}"), Name = "Zarz¹dzanie naprawami", DefaultValue = 0 },
                new AclVerb() { Id = new Guid("{7B129DC6-B78B-4521-AAC9-1282912279E9}"), Name = "Dodawanie nowych napraw", DefaultValue = 0 },
                new AclVerb() { Id = new Guid("{60FF3DB2-DBB4-4B4D-90D0-DFCE413E1935}"), Name = "Dostêp do konfiguracji aplikacji", DefaultValue = 0 },
                new AclVerb() { Id = new Guid("{D060FFBD-4188-443E-862D-8A637CC395D3}"), Name = "Dostêp do konfiguracji wydruków", DefaultValue = 0 },
                new AclVerb() { Id = new Guid("{C463591E-C323-4DC8-8A2D-F79EEF7D1624}"), Name = "Dostêp do konfiguracji numeracji", DefaultValue = 0 }
                );

            context.SaveChanges();

            foreach (var verb in context.AclVerbs)
            {
                context.ACLs.AddOrUpdate(a => a.AclVerbId,
                    new Acl() { AclVerbId = verb.Id, Value = 1 });
            }

            context.SaveChanges();

            context.Roles.AddOrUpdate(n => n.Name,
                new Role() { Name = "Administrator",
                    ACLs = context.ACLs.ToList()
                });

            context.SaveChanges();

            context.Users.AddOrUpdate(u => u.Login,
                new User() { Login = "administrator", IsActive = true, IsAdmin = true, PasswordHash = "123123",
                    RoleId = context.Roles.Single(r => r.Name == "Administrator").Id }
                );

            context.SaveChanges();
        }

    }
}
