using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Entities;

public class JobApplication
{
	public long Id { get; set; } 
	public long JobId { get; set; }
	public long UserId { get; set; }
	public long? CvId { get; set; }

	public string CoverLetter { get; set; } 
	public ApplicationStatus Status { get; set; } 

	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }

	public Job? Job { get; set; } 
	public User? User { get; set; }
	public FreelancerCv? Cv { get; set; }
	public List<Message> Messages { get; set; } 
	public List<ApplicationStatusHistory> StatusHistory { get; set; }

	public JobApplication()
	{
		CoverLetter = string.Empty;
		Status = ApplicationStatus.Pending;
		CreatedAt = DateTime.UtcNow;
		StatusHistory = [];
		Messages = [];
	}
}
