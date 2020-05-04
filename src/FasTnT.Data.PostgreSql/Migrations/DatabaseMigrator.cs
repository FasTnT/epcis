using DbUp;
using System.Reflection;

namespace FasTnT.Data.PostgreSql.Migrations
{
    public static class DatabaseMigrator
    {
        public static void Migrate(string connectionString)
        {
            EnsureDatabase.For.PostgresqlDatabase(connectionString);

            var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .WithVariablesDisabled()
            .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw new System.Exception("Unable to update database");
            }
        }
    }
}
