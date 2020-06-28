using SqlKata;
using SqlKata.Compilers;

namespace ERService.Infrastructure.Repositories
{
    public class SQLQueryBuilder : Query
    {
        public SQLQueryBuilder(string tableName) : base(tableName)
        {
            TableName = tableName;
        }

        public string TableName { get; }        
    }

    public static class SQLQueryBuilderExtensions
    {
        public static string Compile(this Query query, out object[] bindings)
        {
            var compiler = GetCompiler();
            var sqlResult = compiler.Compile(query);
            var queryString = sqlResult.Sql;
            bindings = sqlResult.Bindings.ToArray();

            return queryString;
        }

        private static SqlServerCompiler GetCompiler()
        {
            return new SqlServerCompiler();
        }
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