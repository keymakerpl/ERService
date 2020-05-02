using ERService.Settings.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Dynamic;
using ERService.Infrastructure.Interfaces;
using ERService.Settings.Wrapper;

namespace ERService.Settings.Manager
{
    public class SettingsManager : ISettingsManager
    {
        private readonly ISettingsRepository _settingsRepository;

        public SettingsManager(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        public async Task<dynamic> GetConfigAsync(string configCategory)
        {
            var settings = await _settingsRepository.FindByAsync(s => s.Category == configCategory);

            dynamic config = new ExpandoObject();
            foreach (var setting in settings)
            {
                var wrappedSetting = new SettingWrapper(setting);
                ((IDictionary<string, object>)config).Add(setting.Key, wrappedSetting);
            }

            var result = new ConfigFactory().GetConfig(configCategory, config);

            return result;
        }

        public dynamic GetValue(string value, string valueType)
        {
            var type = Type.GetType(valueType);
            var typeCode = Type.GetTypeCode(type);

            if (type == typeof(Guid))
            {
                var guid = new Guid(value);
                return guid;
            }

            switch (typeCode)
            {
                case TypeCode.Empty:
                    break;
                case TypeCode.Object:
                    break;
                case TypeCode.DBNull:
                    break;
                case TypeCode.Boolean:
                    bool boolean;
                    Boolean.TryParse(value, out boolean);
                    return boolean;
                case TypeCode.Char:
                    break;
                case TypeCode.SByte:
                    break;
                case TypeCode.Byte:
                    break;
                case TypeCode.Int16:
                    break;
                case TypeCode.UInt16:
                    break;
                case TypeCode.Int32:
                    int integer;
                    Int32.TryParse(value, out integer);
                    return integer;
                case TypeCode.UInt32:
                    break;
                case TypeCode.Int64:
                    break;
                case TypeCode.UInt64:
                    break;
                case TypeCode.Single:
                    break;
                case TypeCode.Double:
                    break;
                case TypeCode.Decimal:
                    break;
                case TypeCode.DateTime:
                    DateTime dateTime;
                    DateTime.TryParse(value, out dateTime);
                    return dateTime;
                case TypeCode.String:
                    return value;
                default:
                    return value;
            }
            return value;
        }

        public async void SaveAsync()
        {
            await _settingsRepository.SaveAsync();
        }
    }
}
