using ERService.Business;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ERService.MSSQLDataAccess
{
    public class ERServiceDbContext : DbContext
    {
        //TODO: Make db connection setting in settings
        public ERServiceDbContext() : base("ERServiceDb")
        {

        }

        /// <summary>
        /// Lista
        /// </summary>
        public DbSet<Customer> Customers { get; set; }

        public DbSet<CustomerAddress> CustomerAddresses { get; set; }

        public DbSet<Hardware> Hardwares { get; set; }

        public DbSet<HardwareType> HardwareTypes { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderStatus> OrderStatuses { get; set; }

        public DbSet<OrderType> OrderTypes { get; set; }

        public DbSet<Settings> Settings { get; set; }

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
