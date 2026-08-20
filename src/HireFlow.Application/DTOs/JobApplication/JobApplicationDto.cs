using HireFlow.Application.DTOs.Cv.Responses;

namespace HireFlow.Application.DTOs.JobApplication;

public class JobApplicationDto
{
	public long Id { get; set; }
	public long JobId { get; set; }
	public string JobTitle { get; set; } = string.Empty;
	public string CompanyName { get; set; } = string.Empty;
	public long UserId { get; set; }
	public string ApplicantName { get; set; } = string.Empty;
	public string CoverLetter { get; set; } = string.Empty;
	public string? CvUrl { get; set; }
	public string Status { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }

	public CvDto? Cv { get; set; }
	public List<StatusHistoryDto> StatusHistory { get; set; } = [];
}
