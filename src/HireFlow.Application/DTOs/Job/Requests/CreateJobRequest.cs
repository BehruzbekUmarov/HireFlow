namespace HireFlow.Application.DTOs.Job.Requests;

public class CreateJobRequest
{
	public string Title { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string Category { get; set; } = string.Empty;
	public string Location { get; set; } = string.Empty;
	public decimal Salary { get; set; }
}
