using HireFlow.Api.Extensions;
using HireFlow.Application.Extensions;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Interfaces;
using HireFlow.Infrastructure.Extensions;
using HireFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddCors(options =>
	options.AddPolicy("AllowAngular",
		policy => policy.WithOrigins("http://localhost:4200")
						.AllowAnyHeader()
						.AllowAnyMethod()));

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
		Description = "Enter your JWT token. Example: eyJhbGci..."
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseErrorHandler();

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	await dbContext.Database.MigrateAsync();

	var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
	var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

	await AdminSeeder.SeedAdminUserAsync(db, passwordHasher);
}

app.Run();