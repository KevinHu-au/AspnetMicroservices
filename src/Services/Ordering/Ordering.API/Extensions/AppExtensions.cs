using System;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;
using Serilog;

namespace Ordering.API.Extensions
{
    public static class AppExtensions
    {
        public static WebApplication MigrateDatabase<TContext>(this WebApplication app, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetRequiredService<TContext>();

                try
                {
                    logger.LogInformation("Migrating the database with context {DbContextName}", typeof(TContext));
                    var retry = Policy.Handle<SqlException>()
                                      .WaitAndRetry(
                                            retryCount: 5, 
                                            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                            onRetry: (exception, retryTime, context) => 
                                            {
                                                Log.Error($"Retry {retryTime} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
                                            });
                    retry.Execute(() => InvokeSeeder(seeder, context, services));
                    logger.LogInformation("Migrated the database with context {DbContextName}", typeof(TContext));
                }
                catch (SqlException ex)
                {
                    logger.LogError("data seeding failed with error {Message}", ex.Message);
                    throw;
                }
            }
            return app;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services) where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}
