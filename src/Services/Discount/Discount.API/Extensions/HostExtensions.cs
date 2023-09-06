using Npgsql;

namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        public static IApplicationBuilder MigrateDatabase<TContext>(this IApplicationBuilder host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            using (var scope = host.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating postgresql database");

                    string connectionString = configuration.GetValue<string>("DatabaseSettings:ConnectionString");

                    using var connection = new NpgsqlConnection
                        (connectionString);
                    connection.Open();

                    using var command = new NpgsqlCommand
                    {
                        Connection = connection,
                    };

                    command.CommandText = "DROP TABLE IF EXISTS Coupon";
                    command.ExecuteNonQuery();

                    command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY,
                                                                ProductName VARCHAR(24) NOT NULL,
                                                                Description TEXT,
                                                                Amount INT)";

                    command.ExecuteNonQuery();

                    command.CommandText = @"INSERT INTO Coupon(ProductName, Description, Amount) VALUES
                                                              ('IPhone 12', 'This is seed 1', 200);";
                    command.ExecuteNonQuery();

                    command.CommandText = @"INSERT INTO Coupon(ProductName, Description, Amount) VALUES
                                                              ('Samsung 10', 'This is seed 2', 300);";
                    command.ExecuteNonQuery();

                } catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "An error occured while migrating the postgresql database");
                    
                    if(retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailability);
                    }
                }

                logger.LogInformation("Migrated postgres database successfully");

                return host;
            }
        }
    }
}
