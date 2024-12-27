using CacheLibrary;
using Microsoft.EntityFrameworkCore;
using RedisApp.Api.Model;
using RedisApp.Api.Repositories;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("database");
});

//builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<IProductRepository>(sp=>{
   
    var productRepository = new ProductRepository(sp.GetRequiredService<AppDbContext>());
    var redisService = sp.GetRequiredService<RedisService>();
    return new ProductRepositoryWithCacheDecorator(productRepository, redisService);
});


builder.Services.AddSingleton<RedisService>(sp=>{
    return new RedisService(builder.Configuration["Redis:Url"]);
});
builder.Services.AddSingleton<IDatabase>(sp=>{
    var redisService= sp.GetRequiredService<RedisService>();
    return (IDatabase)redisService.GetDb(0);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
