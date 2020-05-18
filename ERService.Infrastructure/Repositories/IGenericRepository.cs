using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ERService.Infrastructure.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {        
        Task<TEntity> GetByIdAsync(Guid id);
        Task<IEnumerable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate);        
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<List<T>> GetBy<T>(string sqlQuery, object[] parameters);
        Task<IEnumerable<TEntity>> FindByIncludeAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProp);        

        Task<bool> SaveAsync();

        bool HasChanges();
        void Add(TEntity model);
        void Remove(TEntity model);
        void RollBackChanges();
        Task<DbPropertyValues> GetDatabaseValuesAsync(object entity);
        Task ReloadEntities();
        void ReloadNavigationProperty<TElement>(Expression<Func<TEntity, ICollection<TElement>>> navigationProperty) where TElement : class;
    }
}