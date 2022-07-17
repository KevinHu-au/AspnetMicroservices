
using System.Reflection;
using AspnetRunBasics.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(
            new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri(context.Configuration["ElasticConfiguration:Uri"]))
            {
                IndexFormat = $"applogs-{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-logs-{DateTime.UtcNow:yyyy-MM}",
                AutoRegisterTemplate = true,
                NumberOfShards = 2,
                NumberOfReplicas = 1
            })
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
        //Read loging configuration from AppSetting.json
        .ReadFrom.Configuration(context.Configuration);
});

var gatewayAddress = builder.Configuration["ApiSettings:GatewayAddress"];
builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
{
    c.BaseAddress = new Uri(gatewayAddress);
});

builder.Services.AddHttpClient<IBasketService, BasketService>(c =>
{
    c.BaseAddress = new Uri(gatewayAddress);
});

builder.Services.AddHttpClient<IOrderService, OrderService>(c =>
{
    c.BaseAddress = new Uri(gatewayAddress);
});

builder.Services.AddRazorPages();

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
app.Run();
