﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ERService.Infrastructure.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {        
        Task<TEntity> GetByIdAsync(Guid id);
        Task<IEnumerable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> FindByAsync(QueryBuilder<TEntity> queryBuilder);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<Guid[]> GetIDsBy(QueryBuilder<TEntity> queryBuilder);
        IEnumerable<TEntity> FindByInclude(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProp);

        Task<bool> SaveAsync();

        bool HasChanges();
        void Add(TEntity model);
        void Remove(TEntity model);
        void RollBackChanges();
    }
}