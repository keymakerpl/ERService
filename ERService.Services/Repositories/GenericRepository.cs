﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERService.Services.Repositories
{
    public class GenericRepository<TEntity, TContext> :  IGenericRepository<TEntity> 
        where TContext : DbContext
        where TEntity : class
    {
        protected readonly TContext Context;

        protected GenericRepository(TContext context)
        {
            this.Context = context;
        }

        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
            return await Context.Set<TEntity>().FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Context.Set<TEntity>().ToListAsync();
        }

        public async Task SaveAsync()
        {
            await Context.SaveChangesAsync();
        }

        public bool HasChanges()
        {
            return Context.ChangeTracker.HasChanges();
        }

        public void Add(TEntity model)
        {
            Context.Set<TEntity>().Add(model);
        }

        public void Remove(TEntity model)
        {
            Context.Set<TEntity>().Remove(model);
        }
    }
}