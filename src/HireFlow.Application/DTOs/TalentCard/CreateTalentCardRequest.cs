namespace HireFlow.Application.DTOs.TalentCard;

public class CreateTalentCardRequest
{
	public string Title { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string Category { get; set; } = string.Empty;
	public string Skills { get; set; } = string.Empty;
	public decimal HourlyRate { get; set; }
}
