using System;
using StackExchange.Redis;

namespace CacheLibrary;

public class RedisService
{
    private readonly ConnectionMultiplexer _connectionMultiplexer;

    public RedisService(string url)
    {
        _connectionMultiplexer = ConnectionMultiplexer.Connect(url);
    }

    public IDatabase GetDb(int db)
    {
        return _connectionMultiplexer.GetDatabase(db);
    }
}
