using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class FreelancerCvConfiguration : IEntityTypeConfiguration<FreelancerCv>
{
	public void Configure(EntityTypeBuilder<FreelancerCv> builder)
	{
		builder.ToTable("freelancer_cvs");
		builder.HasKey(c => c.Id);
		builder.Property(c => c.Id).ValueGeneratedOnAdd();

		builder.Property(c => c.Title)
			   .HasMaxLength(200)
			   .IsRequired();

		builder.Property(c => c.Summary).HasMaxLength(2000);
		builder.Property(c => c.Skills).HasMaxLength(1000);
		builder.Property(c => c.Experience).HasMaxLength(5000);
		builder.Property(c => c.Education).HasMaxLength(2000);
		builder.Property(c => c.Languages).HasMaxLength(500);
		builder.Property(c => c.PortfolioUrl).HasMaxLength(500);

		builder.HasOne(c => c.User)
			   .WithMany(u => u.Cvs)
			   .HasForeignKey(c => c.UserId)
			   .OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(c => c.Applications)
			   .WithOne(a => a.Cv)
			   .HasForeignKey(a => a.CvId)
			   .OnDelete(DeleteBehavior.SetNull);

		builder.HasIndex(c => c.UserId);
		builder.HasIndex(c => new { c.UserId, c.IsDefault });
	}
}
