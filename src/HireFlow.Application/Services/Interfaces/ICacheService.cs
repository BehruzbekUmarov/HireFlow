namespace HireFlow.Application.Services.Interfaces;

public interface ICacheService
{
	// Get a value by key — returns null if not cached
	Task<T?> GetAsync<T>(string key);

	// Store a value with an expiry time
	Task SetAsync<T>(string key, T value, TimeSpan expiry);

	// Remove a specific key
	Task RemoveAsync(string key);

	// Remove all keys that start with a prefix
	// Used to invalidate all job search results at once
	Task RemoveByPrefixAsync(string prefix);
}