using HealthChecks.UI.Client;
using HireFlow.Api.Middlewares;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Interfaces;
using HireFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HireFlow.Api.Extensions;

public static class WebApplicationExtensions
{
	public static WebApplication UseErrorHandler(
		this WebApplication app)
	{
		app.MapHealthChecks(
			"/health",
			new HealthCheckOptions
			{
				ResponseWriter =
					UIResponseWriter.WriteHealthCheckUIResponse
			});

		app.MapHealthChecks(
			"/health/detail",
			new HealthCheckOptions
			{
				ResponseWriter =
					UIResponseWriter.WriteHealthCheckUIResponse,

				ResultStatusCodes =
				{
					[HealthStatus.Healthy] =
						StatusCodes.Status200OK,

					[HealthStatus.Degraded] =
						StatusCodes.Status200OK,

					[HealthStatus.Unhealthy] =
						StatusCodes.Status503ServiceUnavailable
				}
			});

		app.MapHealthChecks(
			"/health/live",
			new HealthCheckOptions
			{
				Predicate = _ => false
			});

		app.UseMiddleware<ErrorHandlerMiddleware>();

		return app;
	}

	public static WebApplication UseFileStorage(
		this WebApplication app)
	{
		var uploadsPath = Path.Combine(
			app.Environment.ContentRootPath,
			"uploads");

		Directory.CreateDirectory(uploadsPath);

		app.UseDefaultFiles();

		app.UseStaticFiles();

		app.UseStaticFiles(
			new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(uploadsPath),
				RequestPath = "/uploads"
			});

		return app;
	}

	public static async Task InitializeDatabaseAsync(
		this WebApplication app)
	{
		using var scope = app.Services.CreateScope();

		var dbContext =
			scope.ServiceProvider
				.GetRequiredService<AppDbContext>();

		await dbContext.Database.MigrateAsync();

		var db =
			scope.ServiceProvider
				.GetRequiredService<IAppDbContext>();

		var passwordHasher =
			scope.ServiceProvider
				.GetRequiredService<IPasswordHasher>();

		await AdminSeeder.SeedAdminUserAsync(
			db,
			passwordHasher);
	}
}