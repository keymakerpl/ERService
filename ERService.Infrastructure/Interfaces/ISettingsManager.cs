using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ERService.Infrastructure.Interfaces
{
    public interface ISettingsManager<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> FindSettingsAsync(Expression<Func<TEntity, bool>> predicate);
        Task<object> GetConfigAsync(string configCategory);
        dynamic GetValue(string value, string valueType);
        void SaveAsync();
    }
}