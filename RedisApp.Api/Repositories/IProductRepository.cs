using System;
using RedisApp.Api.Model;

namespace RedisApp.Api.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAsync();
    Task<Product> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
}
