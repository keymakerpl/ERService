using ERService.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SqlKata.Compilers;
using SqlKata;

namespace ERService.Infrastructure.Repositories
{
    public class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity>
        where TContext : DbContext
        where TEntity : class
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        protected readonly TContext Context;

        protected GenericRepository(TContext context)
        {
            this.Context = context;

            Context.Database.Log = _logger.Debug;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Context.Set<TEntity>().ToListAsync();
        }

        public virtual async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await Context.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().Where(predicate).ToListAsync();
        }        

        public virtual async Task<IEnumerable<TEntity>> FindByAsync(Query queryBuilder)
        {
            //TODO: Sql compiler
            var compiler = new SqlServerCompiler();
            var sqlResult = compiler.Compile(queryBuilder);
            var query = sqlResult.Sql;
            var bindings = sqlResult.Bindings.ToArray();
            
            return await Context.Database.SqlQuery<TEntity>(query, bindings).ToListAsync();
        }

        public virtual async Task<Guid[]> GetIDsBy(Query queryBuilder)
        {
            //TODO: Sql compiler
            var compiler = new SqlServerCompiler();
            var sqlResult = compiler.Compile(queryBuilder);
            var query = sqlResult.Sql;
            var bindings = sqlResult.Bindings.ToArray();

            var result = await Context.Database.SqlQuery<Guid>(query, bindings).ToListAsync();

            return result.ToArray();
        }

        public virtual IEnumerable<TEntity> FindByInclude(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includeProps)
        {
            var query = GetAllIncluding(includeProps);
            return query.Where(predicate).ToList();
        }

        private IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProps)
        {
            IQueryable<TEntity> queryable = Context.Set<TEntity>();

            return includeProps.Aggregate(queryable, (current, includeProperty) => current.Include(includeProperty));
        }
        
        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProps)
        {            
            IQueryable<TEntity> queryable = Context.Set<TEntity>();

            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }

            //if (includeProperties != null)
            //{
            //    foreach (var includeProperty in includeProperties.Split
            //    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            //    {
            //        query = query.Include(includeProperty);
            //    }
            //}

            if (orderBy != null)
            {
                return orderBy(queryable).ToList();
            }
            else
            {
                var result = includeProps.Aggregate(queryable, (current, includeProperty) => current.Include(includeProperty)).ToList();
                return result;
            }
        }

        public virtual async Task<bool> SaveAsync()
        {
            var objectContextAdapter = Context as IObjectContextAdapter;
            if (objectContextAdapter != null)
            {
                objectContextAdapter.ObjectContext.DetectChanges();
                foreach (ObjectStateEntry entry in objectContextAdapter.ObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added))
                {
                    var added = entry.Entity as IModificationHistory;
                    if (added != null && added.DateAdded == DateTime.MinValue)
                        added.DateAdded = DateTime.Now;
                }

                foreach (ObjectStateEntry entry in objectContextAdapter.ObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Modified))
                {
                    var modified = entry.Entity as IModificationHistory;
                    if (modified != null)
                        modified.DateModified = DateTime.Now;

                    var versioned = entry.Entity as IVersionedRow;
                    if (versioned != null)
                        versioned.RowVersion++;                    
                }
            }

            var savedElements = await Context.SaveChangesAsync();

            return savedElements > 0;
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
            try
            {
                Context.Set<TEntity>().Remove(model);
            }
            catch (Exception ex)
            {
                _logger.Debug(ex);
                _logger.Error(ex);
            }
        }

        public void RollBackChanges()
        {
            var changedEntries = Context.ChangeTracker
                                    .Entries()
                                    .Where(e => e.State != EntityState.Unchanged).ToList();

            foreach (var entry in changedEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Modified:
                        entry.CurrentValues.SetValues(entry.OriginalValues);
                        entry.State = EntityState.Unchanged;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}