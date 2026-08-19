using HealthChecks.UI.Client;
using HireFlow.Api.Middlewares;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HireFlow.Api.Extensions;

public static class WebApplicationExtensions
{
	public static WebApplication UseErrorHandler(this WebApplication app)
	{
		app.MapHealthChecks("/health", new HealthCheckOptions
		{
			ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
		});

		app.MapHealthChecks("/health/detail", new HealthCheckOptions
		{
			ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
			ResultStatusCodes =
	{
		[HealthStatus.Healthy]   = StatusCodes.Status200OK,
		[HealthStatus.Degraded]  = StatusCodes.Status200OK,
		[HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
	}
		});

		app.MapHealthChecks("/health/live", new HealthCheckOptions
		{
			Predicate = _ => false  // just checks if app is running
		});

		app.UseMiddleware<ErrorHandlerMiddleware>();
		return app;
	}
}
