using HireFlow.Application.Common.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace HireFlow.Api.Extensions;

internal static class DependencyInjection
{
	public static IServiceCollection RegisterApi(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		AddOptions(services, configuration);
		AddAuthentication(services, configuration);
		AddSwagger(services);
		AddCors(services);
		AddRateLimiting(services);

		services.AddControllers();
		services.AddEndpointsApiExplorer();
		services.AddProblemDetails();
		services.AddHttpContextAccessor();
		services.AddSignalR();

		return services;
	}

	private static void AddOptions(
		IServiceCollection services,
		IConfiguration configuration)
	{
		services
			.AddOptions<JwtOptions>()
			.Bind(configuration.GetSection(JwtOptions.SectionName))
			.ValidateDataAnnotations()
			.ValidateOnStart();
	}

	private static void AddAuthentication(
		IServiceCollection services,
		IConfiguration configuration)
	{
		var jwtOptions = new JwtOptions();

		configuration
			.GetSection(JwtOptions.SectionName)
			.Bind(jwtOptions);

		if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey))
		{
			throw new InvalidOperationException(
				$"Missing or empty '{JwtOptions.SectionName}:SecretKey' configuration.");
		}

		services
			.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme =
					JwtBearerDefaults.AuthenticationScheme;

				options.DefaultChallengeScheme =
					JwtBearerDefaults.AuthenticationScheme;
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

					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),

					ClockSkew = TimeSpan.Zero
				};
			});

		services.AddAuthorization();
	}

	private static void AddSwagger(
		IServiceCollection services)
	{
		services.AddSwaggerGen(options =>
		{
			options.SwaggerDoc(
				"v1",
				new OpenApiInfo
				{
					Title = "HireFlow API",
					Version = "v1"
				});

			options.AddSecurityDefinition(
				"Bearer",
				new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "Enter your JWT token."
				});

			options.AddSecurityRequirement(
				new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						[]
					}
				});
		});
	}

	private static void AddCors(IServiceCollection services)
	{
		services.AddCors(options =>
		{
			options.AddPolicy("AllowFrontend", policy =>
			{
				policy
					.WithOrigins(
						"http://127.0.0.1:5500",  
						"http://localhost:5500"
					)
					.AllowAnyHeader()
					.AllowAnyMethod()
					.AllowCredentials(); 
			});
		});
	}

	private static void AddRateLimiting(
		IServiceCollection services)
	{
		services.AddRateLimiter(options =>
		{
			options.AddFixedWindowLimiter(
				"auth",
				opt =>
				{
					opt.PermitLimit = 5;
					opt.Window = TimeSpan.FromMinutes(1);
				});
		});
	}
}	