using AspnetRunBasics.Data;
using AspnetRunBasics.Repositories;
using AspnetRunBasics.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// add database dependecy

//// use in-memory database
//builder.Services.AddDbContext<AspnetRunContext>(c =>
//    c.UseInMemoryDatabase("AspnetRunConnection"));
builder.Services.AddDbContext<AspnetRunContext>(c =>
    c.UseSqlServer(builder.Configuration.GetConnectionString("AspnetRunConnection")));

// add repository dependecy
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
  endpoints.MapRazorPages();
});
app.Run();
