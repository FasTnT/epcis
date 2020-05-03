using DbUp;
using System.Reflection;

namespace FasTnT.Data.PostgreSql.Migration
{
    internal static class DatabaseMigrator
    {
        public static void Migrate(string connectionString)
        {
            var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw new System.Exception("Unable to update database");
            }
        }
    }
}
