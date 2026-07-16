using HireFlow.Application.Interfaces;
using HireFlow.Domain.Interfaces;
using HireFlow.Infrastructure.Persistence;
using HireFlow.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HireFlow.Infrastructure.Extensions;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(
		this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<AppDbContext>(options =>
			options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

		services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

		services.AddScoped<IPasswordHasher, PasswordHasher>();
		services.AddScoped<ITokenService, TokenService>();
		services.AddScoped<ICurrentUser, CurrentUser>();

		return services;
	}
}
