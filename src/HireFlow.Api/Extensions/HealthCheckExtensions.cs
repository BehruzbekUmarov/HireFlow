using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HireFlow.Api.Extensions;

public static class HealthCheckExtensions
{
	public static IServiceCollection AddHealthChecksConfiguration(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services
			.AddHealthChecks()
			.AddNpgSql(
				connectionString:
					configuration.GetConnectionString("DefaultConnection")!,
				name: "postgresql",
				failureStatus: HealthStatus.Unhealthy,
				tags: ["database"])
			.AddRedis(
				redisConnectionString:
					configuration.GetConnectionString("Redis")!,
				name: "redis",
				failureStatus: HealthStatus.Degraded,
				tags: ["cache"]);

		return services;
	}
}