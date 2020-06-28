using Prism.Events;
using System;
using System.Linq.Expressions;

namespace ERService.Infrastructure.Events
{
    public class SearchEvent<TEntity> : PubSubEvent<SearchEventArgs<TEntity>>
    {
    }

    public class SearchEventArgs<TEntity>
    {
        public Expression<Func<TEntity, bool>> Predicate { get; set; }
    }
}
