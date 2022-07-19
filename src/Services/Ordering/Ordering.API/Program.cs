using Common.Logging;
using EventBus.Messages.Common;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Ordering.API.EventBusConsumer;
using Ordering.API.Extensions;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SeriLogger.Configure);

// Add services to the container.

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<BasketCheckoutConsumer>();

builder.Services.AddHealthChecks()
                .AddDbContextCheck<OrderContext>()
                .AddRabbitMQ(
                    builder.Configuration["EventBusSettings:HostAddress"],
                    name: "RabbitMQ Health",
                    failureStatus:HealthStatus.Degraded);

//MassTransit-RabbitMq
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<BasketCheckoutConsumer>();
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetValue<string>("EventBusSettings:HostAddress"));
        cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c => 
        {
            c.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
        });
    });
});

// builder.Services.AddMassTransitHostedService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/hc", new HealthCheckOptions()
                            {
                                Predicate = _ => true,
                                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                            });

app.MigrateDatabase<OrderContext>((context, services) => 
{
    var logger = services.GetService<ILogger<OrderContextSeed>>();
    OrderContextSeed.SeedAsync(context, logger).Wait();
});

app.Run();
