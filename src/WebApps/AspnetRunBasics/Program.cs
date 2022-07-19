using AspnetRunBasics.Services;
using Common.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using WebApps.AspnetRunBasics.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SeriLogger.Configure);

builder.Services.AddTransient<LoggingDelegatingHandler>();
var gatewayAddress = builder.Configuration["ApiSettings:GatewayAddress"];
builder.Services.AddHttpClient<ICatalogService, CatalogService>(c => c.BaseAddress = new Uri(gatewayAddress))
                .AddHttpMessageHandler<LoggingDelegatingHandler>()
                .AddPolicyHandler(PolicyHelper.GetRetryPolicy())
                .AddPolicyHandler(PolicyHelper.GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IBasketService, BasketService>(c => c.BaseAddress = new Uri(gatewayAddress))
                .AddHttpMessageHandler<LoggingDelegatingHandler>()
                .AddPolicyHandler(PolicyHelper.GetRetryPolicy())
                .AddPolicyHandler(PolicyHelper.GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IOrderService, OrderService>(c => c.BaseAddress = new Uri(gatewayAddress))
                .AddHttpMessageHandler<LoggingDelegatingHandler>()
                .AddPolicyHandler(PolicyHelper.GetRetryPolicy())
                .AddPolicyHandler(PolicyHelper.GetCircuitBreakerPolicy());

builder.Services.AddRazorPages();

builder.Services.AddHealthChecks()
                .AddUrlGroup(new Uri($"{builder.Configuration["ApiSettings:GatewayAddress"]}"), "Ocelog API Gateway", HealthStatus.Degraded);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});

app.MapHealthChecks("/hc", new HealthCheckOptions()
                            {
                                Predicate = _ => true,
                                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                            });

app.Run();
