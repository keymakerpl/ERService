using SqlKata;

namespace ERService.Infrastructure.Repositories
{
    public class QueryBuilder<TEntity> : Query
    {
        public QueryBuilder() : base(typeof(TEntity).Name)
        {            
        }

        public string TableName { get { return typeof(TEntity).Name; } }        
    }

    public static class SQLOperators
    {
        public const string Greater = ">";
        public const string GreaterOrEqual = ">=";
        public const string Equal = "=";
        public const string Less = "<";
        public const string LessOrEqual = "<=";
    }
}