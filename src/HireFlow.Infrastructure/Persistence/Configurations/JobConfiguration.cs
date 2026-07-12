using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
	public void Configure(EntityTypeBuilder<Job> builder)
	{
		// ---------- Table ----------
		builder.ToTable("jobs");

		// ---------- Primary key ----------
		builder.HasKey(j => j.Id);

		builder.Property(j => j.Id)
			   .ValueGeneratedOnAdd();

		// ---------- Properties ----------
		builder.Property(j => j.Title)
			   .HasMaxLength(200)
			   .IsRequired();

		builder.Property(j => j.Description)
			   .HasMaxLength(5000)
			   .IsRequired();

		builder.Property(j => j.Category)
			   .HasMaxLength(100)
			   .IsRequired();

		builder.Property(j => j.Location)
			   .HasMaxLength(200)
			   .IsRequired();

		// decimal(12,2) = up to 9,999,999,999.99
		// Never use float or double for money — rounding errors
		builder.Property(j => j.Salary)
			   .HasColumnType("decimal(12,2)")
			   .IsRequired();

		builder.Property(j => j.IsActive)
			   .IsRequired()
			   .HasDefaultValue(true);

		builder.Property(j => j.CreatedAt)
			   .IsRequired();

		builder.Property(j => j.UpdatedAt);  // nullable — no IsRequired()

		// ---------- Indexes ----------
		// These three are filtered constantly in your search endpoint
		// Without indexes, PostgreSQL scans every row on every search
		builder.HasIndex(j => j.Category);
		builder.HasIndex(j => j.Location);
		builder.HasIndex(j => j.IsActive);

		// CreatedAt is used for "newest first" sorting
		builder.HasIndex(j => j.CreatedAt);

		// ---------- Relationships ----------
		// Company → Jobs relationship is configured in CompanyConfiguration
		// Jobs → JobApplications:
		builder.HasMany(j => j.JobApplications)
			   .WithOne(a => a.Job)
			   .HasForeignKey(a => a.JobId)
			   .OnDelete(DeleteBehavior.Cascade);
		// Cascade: if a job is deleted, all applications to it are deleted
	}
}
