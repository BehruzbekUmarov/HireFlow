namespace HireFlow.Application.DTOs.TalentCard;

public class TalentCardDto
{
	public long Id { get; set; }
	public long UserId { get; set; }
	public string FreelancerName { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string Category { get; set; } = string.Empty;
	public string Skills { get; set; } = string.Empty;
	public decimal HourlyRate { get; set; }
	public bool IsActive { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
