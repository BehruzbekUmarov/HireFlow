using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
	public void Configure(EntityTypeBuilder<Job> builder)
	{
		builder.ToTable("jobs");

		builder.HasKey(j => j.Id);

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

		builder.Property(j => j.Salary)
			   .HasColumnType("decimal(12,2)")
			   .IsRequired();

		builder.Property(j => j.IsActive)
			   .IsRequired()
			   .HasDefaultValue(true);

		builder.Property(j => j.CreatedAt)
			   .IsRequired();

		builder.Property(j => j.UpdatedAt)
			.IsRequired(false);

		builder.HasIndex(j => j.Category);
		builder.HasIndex(j => j.Location);
		builder.HasIndex(j => j.IsActive);

		builder.HasIndex(j => j.CreatedAt);

		builder.HasMany(j => j.JobApplications)
			   .WithOne(a => a.Job)
			   .HasForeignKey(a => a.JobId)
			   .OnDelete(DeleteBehavior.Cascade);

		builder.HasQueryFilter(j => !j.IsDeleted);
	}
}
