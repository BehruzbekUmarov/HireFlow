namespace HireFlow.Application.DTOs.Cv.Responses;

public class CvDto
{
	public long Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string? Summary { get; set; }
	public string? Skills { get; set; }
	public string? Experience { get; set; }
	public string? Education { get; set; }
	public string? Languages { get; set; }
	public string? PortfolioUrl { get; set; }
	public string? FileUrl { get; set; }
	public int? YearsOfExperience { get; set; }
	public bool IsDefault { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
