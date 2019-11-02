using Xunit;
using System;
using ERService.Settings.Data.Repository;
using ERService.MSSQLDataAccess;
using System.Threading.Tasks;

namespace ERService.Settings.Manager.Tests
{
    public class SettingsManagerTests
    {
        public SettingsManager Manager { get; }

        public SettingsManagerTests()
        {
            Manager = new SettingsManager(new SettingsRepository(new ERServiceDbContext()));
        }

        [Fact()]
        public void GetValueShouldReturnGuid()
        {
            var targetGuid = new Guid("FB6D6CFA-2076-4C31-BD2B-1F70F5F8CD54");

            var actualGuid = Manager.GetValue("FB6D6CFA-2076-4C31-BD2B-1F70F5F8CD54", typeof(Guid).AssemblyQualifiedName);

            Assert.Equal(targetGuid, actualGuid);
        }

        [Fact()]
        public async Task GetConfigAsyncShouldGetNonNullObject()
        {
            var config = await Manager.GetConfigAsync("CompanyInfo");

            Assert.NotNull(config);
        }
    }
}