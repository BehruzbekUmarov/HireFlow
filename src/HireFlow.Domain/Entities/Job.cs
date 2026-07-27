namespace HireFlow.Domain.Entities;

public class Job
{
	public long Id { get; set; } 
	public long CompanyId { get; set; }

	public string Title { get; set; } 
	public string Description { get; set; }
	public string Category { get; set; }
	public string Location { get; set; } 
	public decimal Salary { get; set; }
	public bool IsActive { get; set; }
	public bool IsDeleted { get; set; }      
	public DateTime? ExpiresAt { get; set; }

	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public DateTime? DeletedAt { get; set; }

	public Company? Company { get; set; } 
	public List<JobApplication> JobApplications { get; set; }

	public Job()
	{
		Title = string.Empty;
		Description = string.Empty;
		Category = string.Empty;
		Location = string.Empty;
		Salary = 0;
		IsActive = true;
		IsDeleted = false;
		CreatedAt = DateTime.UtcNow;
		JobApplications = [];
	}
}
