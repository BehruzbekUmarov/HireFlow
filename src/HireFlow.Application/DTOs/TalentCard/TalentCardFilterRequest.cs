namespace HireFlow.Application.DTOs.TalentCard;

public class TalentCardFilterRequest
{
	public string? Keyword { get; set; }
	public string? Category { get; set; }
	public decimal? MinRate { get; set; }
	public decimal? MaxRate { get; set; }
	public int PageNumber { get; set; } = 1;
	public int PageSize { get; set; } = 10;
}
