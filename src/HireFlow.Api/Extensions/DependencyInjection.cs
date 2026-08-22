using HireFlow.Application.Common.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HireFlow.Api.Extensions;

internal static class DependencyInjection
{
	public static IServiceCollection RegisterApi(this IServiceCollection services, IConfiguration configuration)
	{
		AddOptions(services, configuration);
		AddAuthentication(services, configuration);

		services.AddSignalR();

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

	private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
	{
		var jwtOptions = new JwtOptions();
		configuration.GetSection(JwtOptions.SectionName).Bind(jwtOptions);

		if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey))
			throw new InvalidOperationException($"Missing or empty '{JwtOptions.SectionName}:SecretKey' configuration.");

		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = jwtOptions.Issuer,
				ValidAudience = jwtOptions.Audience,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
				ClockSkew = TimeSpan.Zero 
			};
		});

		services.AddAuthorization();
	}
}
