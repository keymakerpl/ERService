using ERService.Business;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ERService.SQLLiteDataAccess
{   
    public class ERServiceDbContext : DbContext
    {
        //TODO: Make db connection setting in settings
        public ERServiceDbContext() : base("ERServiceDb")
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<CustomerAddress> Addresses { get; set; }

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
