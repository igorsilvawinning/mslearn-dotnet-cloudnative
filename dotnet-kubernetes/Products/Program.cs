using Microsoft.Extensions.Http.Resilience;
using Microsoft.EntityFrameworkCore;
using Products.Data;
using Products.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ProductDataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ProductsContext") ?? throw new InvalidOperationException("Connection string 'ProductsContext' not found.")));

builder.Services.AddStandardResilienceHandler();

builder.Services.AddHttpClient<ServiceBeingCalled>(httpClient =>
{
    httpClient.BaseAddress = new Uri("https://service.endpoint/");
}).AddStandardResilienceHandler();

builder.Services.AddStandardResilienceHandler( options => 
{  
    options.RetryOptions.RetryCount = 10;
    options.RetryOptions.BaseDelay = TimeSpan.FromSeconds(1);
});

// Add services to the container.
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapProductEndpoints();

app.UseStaticFiles();

app.CreateDbIfNotExists();

app.Run();
