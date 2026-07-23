namespace HireFlow.Application.Common.Constants;

public static class CacheKeys
{
	public const string JobSearchPrefix = "jobs:search:";

	public static string JobSearch(
		string? keyword,
		string? category,
		string? location,
		decimal? minSalary,
		decimal? maxSalary,
		string sortBy,
		string sortOrder,
		int pageNumber,
		int pageSize)
	{
		return $"{JobSearchPrefix}{keyword}:{category}:{location}" +
			   $":{minSalary}:{maxSalary}:{sortBy}:{sortOrder}" +
			   $":{pageNumber}:{pageSize}";
	}
}