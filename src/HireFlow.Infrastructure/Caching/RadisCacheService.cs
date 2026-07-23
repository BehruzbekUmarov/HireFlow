using HireFlow.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace HireFlow.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
	private readonly IDatabase _db;
	private readonly IServer _server;
	private readonly ILogger<RedisCacheService> _logger;

	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
	{
		_db = redis.GetDatabase();

		var endpoints = redis.GetEndPoints();
		_server = redis.GetServer(endpoints[0]);
		_logger = logger;
	}

	public async Task<T?> GetAsync<T>(string key)
	{
		try
		{
			var value = await _db.StringGetAsync(key);
			if (!value.HasValue) return default;

			return JsonSerializer.Deserialize<T>((string)value!, JsonOptions);
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Redis GET failed for key: {Key}", key);
			return default;
		}
	}

	public async Task SetAsync<T>(string key, T value, TimeSpan expiry)
	{
		try
		{
			var json = JsonSerializer.Serialize(value, JsonOptions);
			await _db.StringSetAsync(key, json, expiry);
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Redis SET failed for key: {Key}", key);
		}
	}

	public async Task RemoveAsync(string key)
	{
		try
		{
			await _db.KeyDeleteAsync(key);
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Redis DELETE failed for key: {Key}", key);
		}
	}

	public async Task RemoveByPrefixAsync(string prefix)
	{
		try
		{
			var keys = _server.Keys(pattern: $"{prefix}*").ToArray();
			if (keys.Length > 0)
				await _db.KeyDeleteAsync(keys);
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Redis prefix DELETE failed for prefix: {Prefix}", prefix);
		}
	}
}