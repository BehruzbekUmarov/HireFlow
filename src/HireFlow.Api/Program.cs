using HireFlow.Api.Extensions;
using HireFlow.Infrastructure.Hubs;
using HireFlow.Application.Extensions;
using HireFlow.Infrastructure.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.CreateBootstrapLogger();

try
{
	var builder = WebApplication.CreateBuilder(args);

	builder.Host.ConfigureSerilog();

	builder.Services.AddHealthChecksConfiguration(
	builder.Configuration);

	// -----------------------------------------
	// Services
	// -----------------------------------------

	builder.Services
		.AddApplication()
		.AddInfrastructure(builder.Configuration)
		.RegisterApi(builder.Configuration);

	var app = builder.Build();

	// -----------------------------------------
	// Middleware
	// -----------------------------------------

	app.UseErrorHandler();

	app.UseSerilogRequestLogging(options =>
	{
		options.MessageTemplate =
			"HTTP {RequestMethod} {RequestPath} → {StatusCode} ({Elapsed:0.0}ms)";
	});

	app.UseFileStorage();

	app.UseSwagger();
	app.UseSwaggerUI();

	app.UseCors("AllowAngular");

	app.UseRateLimiter();

	app.UseAuthentication();
	app.UseAuthorization();

	// -----------------------------------------
	// Endpoints
	// -----------------------------------------

	app.MapControllers();
	app.MapHub<ChatHub>("/hubs/chat");

	// -----------------------------------------
	// Database
	// -----------------------------------------

	await app.InitializeDatabaseAsync();

	// -----------------------------------------
	// Run
	// -----------------------------------------

	await app.RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
	Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
	await Log.CloseAndFlushAsync();
}