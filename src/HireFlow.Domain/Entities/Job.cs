using HireFlow.Domain.Primitives;

namespace HireFlow.Domain.Entities;

public sealed class Job : AggregateRoot
{
	public long CompanyId { get; private set; }

	public string Title { get; private set; } = string.Empty;
	public string Description { get; private set; } = string.Empty;
	public string Category { get; private set; } = string.Empty;
	public string Location { get; private set; } = string.Empty;
	public decimal Salary { get; private set; }
	public bool IsActive { get; private set; } = true;
	public bool IsDeleted { get; private set; }
	public DateTime? ExpiresAt { get; private set; }

	public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
	public DateTime? UpdatedAt { get; private set; }
	public DateTime? DeletedAt { get; private set; }

	public Company? Company { get; set; }
	public List<JobApplication> JobApplications { get; set; } = [];

	private Job()
	{
	}

	public static Job Create(
		Company company,
		string title,
		string description,
		string category,
		string location,
		decimal salary)
	{
		return new Job
		{
			Company = company,
			CompanyId = company.Id,
			Title = title.Trim(),
			Description = description.Trim(),
			Category = category.Trim(),
			Location = location.Trim(),
			Salary = salary,
			IsActive = true,
			CreatedAt = DateTime.UtcNow
		};
	}

	public void UpdateDetails(
		string title,
		string description,
		string category,
		string location,
		decimal salary)
	{
		Title = title.Trim();
		Description = description.Trim();
		Category = category.Trim();
		Location = location.Trim();
		Salary = salary;
		UpdatedAt = DateTime.UtcNow;
	}

	public void Close()
	{
		IsActive = false;
		UpdatedAt = DateTime.UtcNow;
	}

	public void Delete()
	{
		IsDeleted = true;
		IsActive = false;
		DeletedAt = DateTime.UtcNow;
	}
}
