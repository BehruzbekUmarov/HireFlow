using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Interfaces;
using HireFlow.Infrastructure.Caching;
using HireFlow.Infrastructure.Persistence;
using HireFlow.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace HireFlow.Infrastructure.Extensions;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(
		this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<AppDbContext>(options =>
			options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

		services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

		// ---------- Redis ----------
		var redisConnection = configuration.GetConnectionString("Redis")!;

		services.AddSingleton<IConnectionMultiplexer>(
			ConnectionMultiplexer.Connect(redisConnection));

		services.AddSingleton<IConnectionMultiplexer>(_ =>
		{
			var config = ConfigurationOptions.Parse(redisConnection);
			config.AbortOnConnectFail = false; 
			return ConnectionMultiplexer.Connect(config);
		});

		services.AddScoped<ICacheService, RedisCacheService>();

		services.AddScoped<IPasswordHasher, PasswordHasher>();
		services.AddScoped<ITokenService, TokenService>();
		services.AddScoped<ICurrentUser, CurrentUser>();

		return services;
	}
}
