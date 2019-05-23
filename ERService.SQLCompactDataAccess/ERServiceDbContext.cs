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

        /// <summary>
        /// Tutaj ustawiamy jak ma być tworzona baza
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //Fluent API - zakomentowane bo użyjemy atrybutów, a następnie update-migration
            //modelBuilder.Configurations.Add(new FriendConfiguration());

        }
    }


    /// <summary>
    /// Fluent Api cfg example
    /// </summary>
    public class FriendConfiguration : EntityTypeConfiguration<Customer>
    {
        public FriendConfiguration()
        {
            Property(f => f.FirstName)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
