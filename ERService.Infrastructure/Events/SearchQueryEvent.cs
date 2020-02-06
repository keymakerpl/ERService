using ERService.Infrastructure.Repositories;
using Prism.Events;

namespace ERService.Infrastructure.Events
{
    public class SearchQueryEvent<TEntity> : PubSubEvent<SearchQueryEventArgs<TEntity>> where TEntity : class 
    {
    }

    public class SearchQueryEventArgs<TEntity>
    {
        public QueryBuilder<TEntity> QueryBuilder { get; set; }
    }
}
