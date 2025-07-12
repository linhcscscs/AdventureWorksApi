using AdventureWorks.Application.Interface;
using AdventureWorks.Application.Services;
using AdventureWorks.Infrastructure.CacheProvider.BaseCache.Interface;
using AdventureWorks.Infrastructure.CacheProvider.MemCache;
using AdventureWorks.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#region Caching 
builder.Services.AddSingleton<ICacheProvider, MemCacheProvider>();
#endregion
#region Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AdventureWorks2022Context>(options =>
    options.UseSqlServer(connectionString));
#endregion
#region Application Services
builder.Services.AddScoped<IOrderService, OrderService>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
