using HireFlow.Application.DTOs.Job.Responses;
using HireFlow.Domain.Entities;
using System.Linq.Expressions;

namespace HireFlow.Application.Common.Mappings;

public static class JobMapping
{
	public static Expression<Func<Job, JobDetailDto>> ProjectToDetailDto() => j => new JobDetailDto
	{
		Id = j.Id,
		Title = j.Title,
		Description = j.Description,
		CompanyName = j.Company!.Name,
		Category = j.Category,
		Location = j.Location,
		Salary = j.Salary,
		IsActive = j.IsActive,
		CreatedAt = j.CreatedAt,
		UpdatedAt = j.UpdatedAt,
		ApplicationCount = j.JobApplications.Count
	};
}