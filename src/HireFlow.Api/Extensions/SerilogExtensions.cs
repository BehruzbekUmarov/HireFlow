using Serilog;
using Serilog.Events;

namespace HireFlow.Api.Extensions;

public static class SerilogExtensions
{
	public static void ConfigureSerilog(
		this IHostBuilder host)
	{
		host.UseSerilog((context, services, configuration) =>
		{
			configuration
				.MinimumLevel.Information()

				.MinimumLevel.Override(
					"Microsoft",
					LogEventLevel.Warning)

				.MinimumLevel.Override(
					"Microsoft.EntityFrameworkCore",
					LogEventLevel.Warning)

				.MinimumLevel.Override(
					"MassTransit",
					LogEventLevel.Warning)

				.Enrich.FromLogContext()
				.Enrich.WithEnvironmentName()
				.Enrich.WithThreadId()

				.WriteTo.Console(
					outputTemplate:
						"[{Timestamp:HH:mm:ss} {Level:u3}] " +
						"{Message:lj}{NewLine}{Exception}")

				.WriteTo.File(
					path: "logs/hireflow-.log",
					rollingInterval: RollingInterval.Day,
					retainedFileCountLimit: 7,
					outputTemplate:
						"{Timestamp:yyyy-MM-dd HH:mm:ss.fff} " +
						"[{Level:u3}] " +
						"{Message:lj}{NewLine}{Exception}");
		});
	}
}