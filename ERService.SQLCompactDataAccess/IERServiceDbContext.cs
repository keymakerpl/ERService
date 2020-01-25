using System.Data.Entity;
using ERService.Business;

namespace ERService.MSSQLDataAccess
{
    public interface IERServiceDbContext
    {
        DbSet<Acl> ACLs { get; set; }
        DbSet<AclVerb> AclVerbs { get; set; }
        DbSet<CustomerAddress> CustomerAddresses { get; set; }
        DbSet<Customer> Customers { get; set; }
        DbSet<CustomItem> CustomItems { get; set; }
        DbSet<HwCustomItem> HardwareCustomItems { get; set; }
        DbSet<Hardware> Hardwares { get; set; }
        DbSet<HardwareType> HardwareTypes { get; set; }
        DbSet<Numeration> Numeration { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<OrderStatus> OrderStatuses { get; set; }
        DbSet<OrderType> OrderTypes { get; set; }
        DbSet<PrintTemplate> PrintTemplates { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<Setting> Settings { get; set; }
        DbSet<User> Users { get; set; }
    }
}