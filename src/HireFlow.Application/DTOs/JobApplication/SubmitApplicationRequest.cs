namespace HireFlow.Application.DTOs.JobApplication;

public class SubmitApplicationRequest
{
	public string CoverLetter { get; set; } = string.Empty;
	public string? CvUrl { get; set; }
}
