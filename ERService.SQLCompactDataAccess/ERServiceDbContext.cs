using CommonServiceLocator;
using ERService.Business;
using ERService.Infrastructure.Base.Common;
using ERService.MSSQLDataAccess.Migrations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ERService.MSSQLDataAccess
{
    [DbConfigurationType(typeof(ERServiceDbConfiguration))]
    public class ERServiceDbContext : DbContext, IERServiceDbContext
    {
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

        public ERServiceDbContext() : base(ConnectionStringProvider.Current)
        {
            Database.SetInitializer(new ERSCreateDatabaseIfNotExists());            
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ERServiceDbContext, Configuration>());

            if (_config.DatabaseProvider == DatabaseProviders.MySQLServer && !Database.Exists())
            {
                //Database.Initialize(false);
            }
        }
        
        public DbSet<Customer> Customers { get; set; }

        public DbSet<CustomerAddress> CustomerAddresses { get; set; }

        public DbSet<Hardware> Hardwares { get; set; }

        public DbSet<HwCustomItem> HardwareCustomItems { get; set; }

        public DbSet<CustomItem> CustomItems { get; set; }

        public DbSet<HardwareType> HardwareTypes { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderStatus> OrderStatuses { get; set; }

        public DbSet<OrderType> OrderTypes { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<Numeration> Numeration { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Acl> ACLs { get; set; }

        public DbSet<AclVerb> AclVerbs { get; set; }

        public DbSet<PrintTemplate> PrintTemplates { get; set; }

        /// <summary>
        /// Tutaj ustawiamy jak ma być tworzona baza
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //Fluent API - zakomentowane bo użyjemy atrybutów, a następnie update-migration
            //modelBuilder.Configurations.Add(new CustomerConfiguration());
        }
    }

    /// <summary>
    /// Fluent Api cfg example
    /// </summary>
    public class CustomerConfiguration : EntityTypeConfiguration<Customer>
    {
        public CustomerConfiguration()
        {
            Property(f => f.FirstName)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
