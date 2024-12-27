using System;
using System.Text.Json;
using CacheLibrary;
using RedisApp.Api.Model;
using StackExchange.Redis;

namespace RedisApp.Api.Repositories;

public class ProductRepositoryWithCacheDecorator : IProductRepository
{
    private const string key = "productCaches";
    private readonly IProductRepository _productRepository;
    private readonly RedisService _redisService;
    private readonly IDatabase _cacheRepository;

    public ProductRepositoryWithCacheDecorator(IProductRepository productRepository, RedisService redisService)
    {
        _productRepository = productRepository;
        _redisService = redisService;
        _cacheRepository = _redisService.GetDb(1);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        var newProduct = await _productRepository.CreateAsync(product);

        if (await _cacheRepository.KeyExistsAsync(key))
            await _cacheRepository.HashSetAsync(key, product.Id, JsonSerializer.Serialize(newProduct));

        return newProduct;
    }

    public async Task<List<Product?>> GetAsync()
    {
        if (!await _cacheRepository.KeyExistsAsync(key))
            return await LoadToCacheFromDbAsync();

        var products = new List<Product>();

        foreach (var item in await _cacheRepository.HashGetAllAsync(key))
        {
            products.Add(JsonSerializer.Deserialize<Product>(item.Value));
        }

        return products;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        if (_cacheRepository.KeyExists(key))
        {
            var product = await _cacheRepository.HashGetAsync(key, id);

            return product.HasValue ? JsonSerializer.Deserialize<Product>(product) : null;
        }

        var products = await LoadToCacheFromDbAsync();
        return products.FirstOrDefault(product => product.Id == id);
    }

    private async Task<List<Product>> LoadToCacheFromDbAsync()
    {
        var products = await _productRepository.GetAsync();

        products.ForEach(product =>
        {
            _cacheRepository.HashSetAsync(key, product.Id, JsonSerializer.Serialize(product));
        });

        return products;
    }
}
