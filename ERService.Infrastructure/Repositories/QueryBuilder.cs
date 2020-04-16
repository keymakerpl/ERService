using SqlKata;

namespace ERService.Infrastructure.Repositories
{
    public class QueryBuilder : Query
    {
        private readonly string _tableName;

        public QueryBuilder(string tableName) : base(tableName)
        {
            _tableName = tableName;
        }

        public string TableName => _tableName;
    }

    public static class SQLOperators
    {
        public const string Greater = ">";
        public const string GreaterOrEqual = ">=";
        public const string Equal = "=";
        public const string NotEqual = "<>";
        public const string Less = "<";
        public const string LessOrEqual = "<=";
    }
}