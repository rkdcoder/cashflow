using CashFlow.Shared.Abstractions;
using CashFlow.Shared.Exceptions.Application;
using Npgsql;

namespace CashFlow.Infrastructure.Shared.Data
{
    public class DatabaseHelper : IDatabaseHelper
    {
        public void EnsureDatabaseExists(string connectionString)
        {
            const int maxAttempts = 10;
            const int delayMs = 20000;
            int attempt = 0;
            Exception? lastEx = null;

            while (attempt++ < maxAttempts)
            {
                var builder = new NpgsqlConnectionStringBuilder(connectionString);
                var database = builder.Database;

                try
                {
                    if (string.IsNullOrWhiteSpace(database))
                        throw new ArgumentException("The database name cannot be null or empty in the connection string.", nameof(connectionString));

                    using var connection = new NpgsqlConnection(builder.ToString());
                    connection.Open();

                    using var cmd = connection.CreateCommand();
                    cmd.CommandText = "SELECT 1";
                    cmd.ExecuteScalar();

                    System.Diagnostics.Debug.WriteLine($"Database '{database}' already exists and is accessible.");
                    return;
                }
                catch (PostgresException pex) when (pex.SqlState == "3D000")
                {
                    try
                    {
                        var adminBuilder = new NpgsqlConnectionStringBuilder(connectionString);
                        adminBuilder.Database = "postgres";
                        using var adminConn = new NpgsqlConnection(adminBuilder.ToString());
                        adminConn.Open();

                        using var createCmd = adminConn.CreateCommand();
                        createCmd.CommandText = $"CREATE DATABASE \"{database}\"";
                        createCmd.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"Database '{database}' created.");
                        return;
                    }
                    catch (Exception ex)
                    {
                        lastEx = ex;
                        System.Diagnostics.Debug.WriteLine($"[Attempt {attempt}]  Failed to create the database: {ex.Message}");
                        Thread.Sleep(delayMs);
                    }
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                    System.Diagnostics.Debug.WriteLine($"[Attempt {attempt}] PostgreSQL is not ready yet or database access failed: {ex.Message}");
                    Thread.Sleep(delayMs);
                }
            }
            throw new DatabaseInitializationException("Could not ensure database exists after several attempts. Perhaps Docker container is not ready.", lastEx);
        }
    }
}