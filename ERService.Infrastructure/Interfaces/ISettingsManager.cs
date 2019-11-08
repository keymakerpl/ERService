using ERService.Business;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ERService.Infrastructure.Interfaces
{
    public interface ISettingsManager
    {
        Task<IEnumerable<Setting>> FindSettingsAsync(Expression<Func<Setting, bool>> predicate);
        Task<object> GetConfigAsync(string configCategory);
        dynamic GetValue(string value, string valueType);
        void SaveAsync();
    }
}