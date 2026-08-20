using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
	public void Configure(EntityTypeBuilder<JobApplication> builder)
	{
		builder.ToTable("job_applications");

		builder.HasKey(a => a.Id);

		builder.Property(a => a.Id)
			   .ValueGeneratedOnAdd();

		builder.Property(a => a.CoverLetter)
			   .HasMaxLength(3000)
			   .IsRequired();

		builder.Property(a => a.Status)
			   .HasConversion<string>()
			   .HasMaxLength(20)
			   .IsRequired();

		builder.Property(a => a.CreatedAt)
			   .IsRequired();

		builder.Property(a => a.UpdatedAt)
			.IsRequired(false);

		builder.HasIndex(a => new { a.JobId, a.UserId })
			   .IsUnique();

		builder.HasOne(a => a.User)
			   .WithMany(u => u.JobApplications)
			   .HasForeignKey(a => a.UserId)
			   .OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(a => a.StatusHistory)
			   .WithOne(h => h.JobApplication)
			   .HasForeignKey(h => h.ApplicationId)
			   .OnDelete(DeleteBehavior.Cascade);
	}
}
