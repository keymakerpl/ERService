using System.Threading.Tasks;

namespace ERService.Infrastructure.Interfaces
{
    public interface ISettingsManager
    {
        Task<object> GetConfigAsync(string configCategory);
        dynamic GetValue(string value, string valueType);
        void SaveAsync();
    }
}