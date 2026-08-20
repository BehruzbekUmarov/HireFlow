using HireFlow.Domain.Entities.Base;

namespace HireFlow.Domain.Entities;

public class FreelancerCv : EntityBase
{
	public long UserId { get; set; }
	public string Title { get; set; } 
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

	public User? User { get; set; }
	public List<JobApplication> Applications { get; set; } 

	public FreelancerCv()
	{
		Title = string.Empty;
		IsDefault = false;
		CreatedAt = DateTime.UtcNow;
		Applications = []; 
	}
}
