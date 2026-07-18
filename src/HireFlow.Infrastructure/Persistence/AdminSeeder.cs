using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrastructure.Persistence;

public static class AdminSeeder
{
	public static async Task SeedAdminUserAsync(
		IAppDbContext db,
		IPasswordHasher passwordHasher)
	{
		var adminExists = await db.Users
			.FirstOrDefaultAsync(u => u.Role == UserRole.Admin);

		if (adminExists is not null) return;

		var admin = new User
		{
			Email = "admin@hireflow.com",
			PasswordHash = passwordHasher.Hash("Admin123!"),
			FullName = "System Admin",
			Role = UserRole.Admin,
			CreatedAt = DateTime.UtcNow,
			JobApplications = [],
			RefreshTokens = []
		};

		db.Users.Add(admin);
		await db.SaveChangesAsync();
	}
}
