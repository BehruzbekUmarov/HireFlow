using HireFlow.Application.Common.Configurations;

namespace HireFlow.Api.Extensions;

internal static class DependencyInjection
{
	public static IServiceCollection RegisterApi(this IServiceCollection services, IConfiguration configuration)
	{
		AddOptions(services, configuration);

		return services;
	}

	private static void AddOptions(IServiceCollection services, IConfiguration configuration)
	{
		services
		   .AddOptions<JwtOptions>()
		   .Bind(configuration.GetSection(JwtOptions.SectionName))
		   .ValidateDataAnnotations()
		   .ValidateOnStart();
	}
}
