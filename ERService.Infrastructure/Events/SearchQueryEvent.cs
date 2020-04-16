using Prism.Events;
using SqlKata;

namespace ERService.Infrastructure.Events
{
    public class SearchQueryEvent : PubSubEvent<SearchQueryEventArgs>
    {
    }

    public class SearchQueryEventArgs
    {
        public Query QueryBuilder { get; set; }
    }
}
