using System;
using AspnetRunBasics.Data;

namespace AspnetRunBasics.Extensions
{
  public static class AppExtensions
  {
    public static void SeedData(this WebApplication app)
    {
      using (var scope = app.Services.CreateScope())
      {
        var services = scope.ServiceProvider;
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();

        try
        {
          var aspnetRunContext = services.GetRequiredService<AspnetRunContext>();
          AspnetRunContextSeed.SeedAsync(aspnetRunContext, loggerFactory).Wait();
        }
        catch (Exception exception)
        {
          var logger = loggerFactory.CreateLogger<Program>();
          logger.LogError(exception, "An error occurred seeding the DB.");
        }
      }
    }
  }
}
