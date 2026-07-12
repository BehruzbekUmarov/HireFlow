namespace HireFlow.Application.DTOs.Job;

public class JobSummaryDto
{
	public long Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string CompanyName { get; set; } = string.Empty;
	public string Category { get; set; } = string.Empty;
	public string Location { get; set; } = string.Empty;
	public decimal Salary { get; set; }
	public bool IsActive { get; set; }
	public DateTime CreatedAt { get; set; }
	public int ApplicationCount { get; set; }
}
