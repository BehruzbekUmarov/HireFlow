using HireFlow.Application.DTOs.JobApplication;
using HireFlow.Domain.Entities;
using System.Linq.Expressions;

namespace HireFlow.Application.Common.Mappings;

public static class JobApplicationMapping
{
	public static Expression<Func<JobApplication, JobApplicationDto>> ProjectToDto() => a => new JobApplicationDto
	{
		Id = a.Id,
		JobId = a.JobId,
		JobTitle = a.Job!.Title,
		CompanyName = a.Job.Company!.Name,
		UserId = a.UserId,
		ApplicantName = a.User!.FullName,
		CoverLetter = a.CoverLetter,
		CvUrl = a.CvUrl,
		Status = a.Status.ToString(),
		CreatedAt = a.CreatedAt,
		UpdatedAt = a.UpdatedAt,
		StatusHistory = a.StatusHistory
			.OrderBy(h => h.ChangedAt)
			.Select(h => new StatusHistoryDto
			{
				OldStatus = h.OldStatus.ToString(),
				NewStatus = h.NewStatus.ToString(),
				ChangedAt = h.ChangedAt
			}).ToList()
	};
}