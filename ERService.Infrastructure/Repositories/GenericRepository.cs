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
using System.Collections.ObjectModel;

namespace ERService.Infrastructure.Repositories
{
    public class ParameterCollection<Expression> : Collection<Expression>
    {
    }

    public class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity>
        where TContext : DbContext
        where TEntity : class
    {
        protected readonly TContext Context;

        protected GenericRepository(TContext context)
        {
            this.Context = context;
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

        public virtual async Task<IEnumerable<TEntity>> FindByAsync(QueryBuilder<TEntity> queryBuilder)
        {
            var compiler = new SqlServerCompiler();
            var sqlResult = compiler.Compile(queryBuilder);
            var query = sqlResult.Sql;
            var bindings = sqlResult.Bindings.ToArray();
            
            return await Context.Database.SqlQuery<TEntity>(query, bindings).ToListAsync();
        }

        public virtual async Task<Guid[]> GetIDsBy(QueryBuilder<TEntity> queryBuilder)
        {
            queryBuilder.Select($"{queryBuilder.TableName}.{"Id"}");

            var compiler = new SqlServerCompiler();
            var sqlResult = compiler.Compile(queryBuilder);
            var query = sqlResult.Sql;
            var bindings = sqlResult.Bindings.ToArray();

            Console.WriteLine($"[DEBUG] {query}");

            var result = await Context.Database.SqlQuery<Guid>(query, bindings).ToListAsync();

            return result.ToArray();
        }

        public virtual IEnumerable<TEntity> FindByInclude(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includeProps)
        {
            var query = GetAllIncluding(includeProps);
            return query.Where(predicate).ToList();
        }

        public IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProps)
        {
            IQueryable<TEntity> queryable = Context.Set<TEntity>();

            return includeProps.Aggregate(queryable, (current, includeProperty) => current.Include(includeProperty));
        }

        public virtual async Task SaveAsync()
        {

#if DEBUG
            Context.Database.Log = Console.Write;
#endif

            var objectContextAdapter = Context as IObjectContextAdapter;
            if (objectContextAdapter != null)
            {
                objectContextAdapter.ObjectContext.DetectChanges();
                foreach (ObjectStateEntry entry in objectContextAdapter.ObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Modified))
                {
                    var v = entry.Entity as IVersionedRow;
                    if (v != null)
                        v.RowVersion++;
                }
            }

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