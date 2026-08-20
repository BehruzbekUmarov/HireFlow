using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Domain.Interfaces;

public interface IAppDbContext
{
	DbSet<User> Users { get; }
	DbSet<Company> Companies { get; }
	DbSet<Job> Jobs { get; }
	DbSet<JobApplication> JobApplications { get; }
	DbSet<ApplicationStatusHistory> ApplicationStatusHistories { get; }
	DbSet<FreelancerCv> FreelancerCvs { get; }
	DbSet<RefreshToken> RefreshTokens { get; }
	DbSet<PasswordResetToken> PasswordResetTokens { get; }

	int SaveChanges();

	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
