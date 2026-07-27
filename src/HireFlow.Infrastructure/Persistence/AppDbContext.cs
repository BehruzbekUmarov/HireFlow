using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
	{
		//Database.Migrate();
	}

	public DbSet<User> Users { get; set; }
	public DbSet<RefreshToken> RefreshTokens { get; set; }
	public DbSet<Job> Jobs { get; set; }
	public DbSet<JobApplication> JobApplications { get; set; }
	public DbSet<Company> Companies { get; set; }
	public DbSet<ApplicationStatusHistory> ApplicationStatusHistories { get; set; }
	public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
	}
}
