using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;

namespace ERService.Settings.Data.Repository
{
    public class SettingsRepository : GenericRepository<Setting, ERServiceDbContext>, ISettingsRepository
    {
        public SettingsRepository(ERServiceDbContext context) : base(context)
        {
        }
    }
}
