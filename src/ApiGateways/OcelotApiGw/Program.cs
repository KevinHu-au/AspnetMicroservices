
using Microsoft.VisualBasic.CompilerServices;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;
using Common.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((hostingContext, config) => 
{
    config.AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true);
});


builder.Host.UseSerilog(SeriLogger.Configure);
// builder.Host.ConfigureLogging((hostingContext, loggingBuilder) => 
// {
//     loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
//     loggingBuilder.AddConsole();
//     loggingBuilder.AddDebug();
// });

builder.Services
       .AddOcelot()
       .AddCacheManager(settings => settings.WithDictionaryHandle());

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Hello World!");
    });
});

await app.UseOcelot();

app.Run();
