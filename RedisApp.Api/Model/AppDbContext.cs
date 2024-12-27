using System;
using Microsoft.EntityFrameworkCore;

namespace RedisApp.Api.Model;

public class AppDbContext:DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
        
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //dummy data for inmemory db
        modelBuilder.Entity<Product>().HasData(
            new Product{Id=1, Name="car", Price=56},
            new Product{Id=2, Name="pencil", Price=5},
            new Product{Id=3, Name="glass", Price=6}        
        );
    }

}
