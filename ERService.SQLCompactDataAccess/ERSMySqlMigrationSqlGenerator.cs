using MySql.Data.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;

namespace ERService.MSSQLDataAccess
{
    internal class ERSMySqlMigrationSqlGenerator : MySqlMigrationSqlGenerator
    {
        protected override MigrationStatement Generate(CreateIndexOperation op)
        {
            var u = new MigrationStatement();
            string unique = (op.IsUnique ? "UNIQUE" : ""), columns = "";
            foreach (var col in op.Columns)
            {
                columns += ($"`{col}` DESC{(op.Columns.IndexOf(col) < op.Columns.Count - 1 ? ", " : "")}");
            }
            u.Sql = $"CREATE {unique} INDEX `{op.Name}` ON `{TrimSchemaPrefix(op.Table)}` ({columns}) USING BTREE";
            return u;            
        }

        private string TrimSchemaPrefix(string table)
        {
            if (table.StartsWith("dbo."))
                return table.Replace("dbo.", "");
            return table;
        }

        public override IEnumerable<MigrationStatement> Generate(IEnumerable<MigrationOperation> migrationOperations, string providerManifestToken)
        {
            IEnumerable<MigrationStatement> res = base.Generate(migrationOperations, providerManifestToken);
            foreach (MigrationStatement ms in res)
            {
                ms.Sql = ms.Sql.Replace("dbo.", "").Replace("default ''", String.Empty).Replace("newsequentialid()", "(uuid())");
#if DEBUG
                Console.WriteLine($"[DEBUG] Generated statement AFTER replace: {ms.Sql}");
#endif
            }

            return res;
        }
    }
}
