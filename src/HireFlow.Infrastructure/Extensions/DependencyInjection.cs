using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Interfaces;
using HireFlow.Infrastructure.Implementations.Caching;
using HireFlow.Infrastructure.Implementations.Documents;
using HireFlow.Infrastructure.Implementations.Email;
using HireFlow.Infrastructure.Implementations.Storage;
using HireFlow.Infrastructure.Messaging;
using HireFlow.Infrastructure.Persistence;
using HireFlow.Infrastructure.Security;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;
using StackExchange.Redis;

namespace HireFlow.Infrastructure.Extensions;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(
		this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<AppDbContext>(options =>
			options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

		services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

		// ---------- Redis ----------
		var redisConnection = configuration.GetConnectionString("Redis")!;

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
		services.AddScoped<IFileStorageService, LocalFileStorageService>();
		services.AddScoped<ICvPdfService, CvPdfService>();

		// ---------- Email ----------
		services.AddHttpClient<IEmailService, ResendEmailService>();

		// ---------- RabbitMQ via MassTransit ----------
		services.AddMassTransit(x =>
		{
			x.AddConsumer<ApplicationStatusChangedConsumer>();
			x.AddConsumer<PasswordResetRequestedConsumer>();

			x.UsingRabbitMq((context, cfg) =>
			{
				cfg.Host(configuration["RabbitMQ:Host"], "/", h =>
				{
					h.Username(configuration["RabbitMQ:Username"]!);
					h.Password(configuration["RabbitMQ:Password"]!);
				});

				cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

				cfg.ConfigureEndpoints(context);
			});
		});

		QuestPDF.Settings.License = LicenseType.Community;

		return services;
	}
}
