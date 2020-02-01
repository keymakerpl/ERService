using SqlKata;

namespace ERService.Infrastructure.Repositories
{
    public class QueryBuilder<TEntity> : Query
    {
        public QueryBuilder() : base(typeof(TEntity).Name)
        {            
        }
    }
}