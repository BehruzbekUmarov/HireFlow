namespace HireFlow.Application.DTOs.JobApplication.Requests;

public class SubmitApplicationRequest
{
	public string CoverLetter { get; set; } = string.Empty;
	public long? CvId { get; set; }
}
