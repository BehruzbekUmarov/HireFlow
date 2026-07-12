using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
	public void Configure(EntityTypeBuilder<JobApplication> builder)
	{
		// ---------- Table ----------
		builder.ToTable("job_applications");

		// ---------- Primary key ----------
		builder.HasKey(a => a.Id);

		builder.Property(a => a.Id)
			   .ValueGeneratedOnAdd();

		// ---------- Properties ----------
		builder.Property(a => a.CoverLetter)
			   .HasMaxLength(3000)
			   .IsRequired();

		builder.Property(a => a.CvUrl)
			   .HasMaxLength(500);  // nullable, no IsRequired()

		// Store as string: "Pending", "Reviewed", "Accepted", "Rejected"
		builder.Property(a => a.Status)
			   .HasConversion<string>()
			   .HasMaxLength(20)
			   .IsRequired();

		builder.Property(a => a.CreatedAt)
			   .IsRequired();

		builder.Property(a => a.UpdatedAt);  // nullable — no IsRequired()

		// ---------- Indexes ----------
		// The most important index in the whole schema:
		// Composite unique index on (JobId + UserId)
		// This enforces: one freelancer can only apply to the same job ONCE
		// Even if your service layer has a bug, the DB physically rejects the duplicate
		builder.HasIndex(a => new { a.JobId, a.UserId })
			   .IsUnique();

		// ---------- Relationships ----------
		// Job → JobApplications is configured in JobConfiguration

		// User → JobApplications:
		builder.HasOne(a => a.User)
			   .WithMany(u => u.JobApplications)
			   .HasForeignKey(a => a.UserId)
			   .OnDelete(DeleteBehavior.Cascade);
		// Cascade: if a user is deleted, their applications are deleted too

		// JobApplication → StatusHistory:
		builder.HasMany(a => a.StatusHistory)
			   .WithOne(h => h.JobApplication)
			   .HasForeignKey(h => h.ApplicationId)
			   .OnDelete(DeleteBehavior.Cascade);
		// Cascade: if an application is deleted, its history is deleted too
	}
}
