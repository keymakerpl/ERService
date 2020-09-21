using SqlKata;
using SqlKata.Compilers;

namespace ERService.Infrastructure.Repositories
{
    public static class SQLQueryBuilder
    {
        public static Query CreateQuery()
        {
            return new Query();
        } 

        public static Query CreateQuery(string tableName)
        {
            return new Query(tableName);
        }
    }

    public static class SQLQueryBuilderExtensions
    {
        public static SQLQuery Compile(this Query query)
        {
            var compiler = GetCompiler();
            var sqlResult = compiler.Compile(query);
            var queryString = sqlResult.Sql;
            var bindings = sqlResult.Bindings.ToArray();

            return new SQLQuery(queryString, bindings);
        }

        private static SqlServerCompiler GetCompiler()
        {
            return new SqlServerCompiler();
        }
    }

    public class SQLQuery
    {
        public SQLQuery(string query, object[] parameters)
        {
            Query = query;
            Parameters = parameters;
        }

        public string Query { get;}
        public object[] Parameters { get;}
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