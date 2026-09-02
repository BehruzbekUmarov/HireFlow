using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HireFlow.Tests.Common;

public class TestAppDbContext : DbContext, IAppDbContext
{
	public TestAppDbContext(DbContextOptions<TestAppDbContext> options)
		: base(options)
	{
	}

	public DbSet<User> Users { get; set; } = null!;
	public DbSet<Company> Companies { get; set; } = null!;
	public DbSet<Job> Jobs { get; set; } = null!;
	public DbSet<JobApplication> JobApplications { get; set; } = null!;
	public DbSet<ApplicationStatusHistory> ApplicationStatusHistories { get; set; } = null!;
	public DbSet<FreelancerCv> FreelancerCvs { get; set; } = null!;
	public DbSet<Message> Messages { get; set; } = null!;
	public DbSet<TalentCard> TalentCards { get; set; } = null!;
	public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
	public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;
}

public static class TestDbContextFactory
{
	public static TestAppDbContext Create()
	{
		var options = new DbContextOptionsBuilder<TestAppDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(w =>
				w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;

		return new TestAppDbContext(options);
	}
}