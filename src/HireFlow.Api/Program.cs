using HireFlow.Api.Extensions;
using HireFlow.Application.Extensions;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Interfaces;
using HireFlow.Infrastructure.Extensions;
using HireFlow.Infrastructure.Hubs;
using HireFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.CreateBootstrapLogger();

try
{
	var builder = WebApplication.CreateBuilder(args);

	builder.Host.ConfigureSerilog(); 

	builder.Services
		.AddApplication()
		.AddInfrastructure(builder.Configuration)
		.RegisterApi(builder.Configuration);

	builder.Services.AddRateLimiter(options =>
		options.AddFixedWindowLimiter("auth", opt =>
		{
			opt.PermitLimit = 5;
			opt.Window = TimeSpan.FromMinutes(1);
		}));

	builder.Services.AddControllers();
	builder.Services.AddEndpointsApiExplorer();

	builder.Services.AddSwaggerGen(options =>
	{
		options.SwaggerDoc("v1", new OpenApiInfo { Title = "HireFlow API", Version = "v1" });
		options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
		{
			Name = "Authorization",
			Type = SecuritySchemeType.Http,
			Scheme = "Bearer",
			BearerFormat = "JWT",
			In = ParameterLocation.Header,
			Description = "Enter your JWT token."
		});
		options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

	builder.Services.AddProblemDetails();
	builder.Services.AddHttpContextAccessor();

	builder.Services.AddCors(options =>
	options.AddPolicy("AllowAngular",
		policy => policy
			.WithOrigins("http://localhost:4200")
			.AllowAnyHeader()
			.AllowAnyMethod()
			.AllowCredentials())); 

	builder.Services.AddHealthChecksConfiguration(builder.Configuration);

	var app = builder.Build();

	app.UseErrorHandler();
	app.UseSerilogRequestLogging(options =>
	{
		options.MessageTemplate =
			"HTTP {RequestMethod} {RequestPath} → {StatusCode} ({Elapsed:0.0}ms)";
	});

	var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "uploads");
	Directory.CreateDirectory(uploadsPath);

	app.UseDefaultFiles();   

	app.UseStaticFiles();

	app.UseStaticFiles(new StaticFileOptions
	{
		FileProvider = new PhysicalFileProvider(uploadsPath),
		RequestPath = "/uploads"
	});

	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	app.UseRateLimiter();
	app.UseAuthentication();
	app.UseAuthorization();
	app.MapControllers();
	app.MapHub<ChatHub>("/hubs/chat");

	using (var scope = app.Services.CreateScope())
	{
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		await dbContext.Database.MigrateAsync();

		var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
		var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

		await AdminSeeder.SeedAdminUserAsync(db, passwordHasher);
	}

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