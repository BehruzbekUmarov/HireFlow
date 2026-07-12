using HireFlow.Application.Interfaces;
using HireFlow.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HireFlow.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddScoped<IAuthService, AuthService>();
		services.AddScoped<IJobService, JobService>();
		services.AddScoped<IJobApplicationService, JobApplicationService>();
		services.AddScoped<IAdminService, AdminService>();
		return services;
	}
}
