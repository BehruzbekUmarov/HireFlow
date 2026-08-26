namespace HireFlow.Application.Services.Interfaces;

public interface ICacheService
{
	Task<T?> GetAsync<T>(string key);
	Task SetAsync<T>(string key, T value, TimeSpan expiry);
	Task RemoveAsync(string key);
	Task RemoveByPrefixAsync(string prefix);
}