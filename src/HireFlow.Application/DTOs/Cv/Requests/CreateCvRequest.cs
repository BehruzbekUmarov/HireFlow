namespace HireFlow.Application.DTOs.Cv.Requests;

public class CreateCvRequest
{
	public string Title { get; set; } = string.Empty;
	public string? Summary { get; set; }
	public string? Skills { get; set; }
	public string? Experience { get; set; }
	public string? Projects { get; set; }
	public string? Education { get; set; }
	public string? Languages { get; set; }
	public string? PortfolioUrl { get; set; }
	public int? YearsOfExperience { get; set; }
	public bool IsDefault { get; set; } = false;
}