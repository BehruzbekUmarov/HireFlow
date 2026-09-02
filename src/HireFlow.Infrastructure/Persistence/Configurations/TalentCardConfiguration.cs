using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class TalentCardConfiguration : IEntityTypeConfiguration<TalentCard>
{
	public void Configure(EntityTypeBuilder<TalentCard> builder)
	{
		builder.ToTable("talent_cards");
		builder.HasKey(t => t.Id);
		builder.Property(t => t.Id).ValueGeneratedOnAdd();

		builder.Property(t => t.Title)
			   .HasMaxLength(200)
			   .IsRequired();

		builder.Property(t => t.Description)
			   .HasMaxLength(3000)
			   .IsRequired();

		builder.Property(t => t.Category)
			   .HasMaxLength(100)
			   .IsRequired();

		builder.Property(t => t.Skills)
			   .HasMaxLength(1000)
			   .IsRequired();

		builder.Property(t => t.HourlyRate)
			   .HasPrecision(18, 2);

		builder.Property(t => t.IsActive)
			   .HasDefaultValue(true);

		builder.HasOne(t => t.User)
			   .WithMany(u => u.TalentCards)
			   .HasForeignKey(t => t.UserId)
			   .OnDelete(DeleteBehavior.Cascade);

		builder.HasIndex(t => t.UserId);
		builder.HasIndex(t => t.Category);
		builder.HasIndex(t => t.IsActive);
	}
}
