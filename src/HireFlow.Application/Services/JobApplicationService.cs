using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.JobApplication;
using HireFlow.Application.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Services;

public class JobApplicationService : IJobApplicationService
{
	private readonly IAppDbContext _db;

	public JobApplicationService(IAppDbContext db)
	{
		_db = db;
	}

	public async Task<JobApplicationDto> SubmitAsync(long jobId, long userId, SubmitApplicationRequest request)
	{
		var job = await _db.Jobs.FindAsync(jobId)
			?? throw new InvalidOperationException("Job not found.");

		if (!job.IsActive)
			throw new InvalidOperationException("This job is no longer accepting applications.");

		// Check duplicate — even though the DB has a unique index,
		// we catch it here first to return a friendly message
		var alreadyApplied = await _db.JobApplications
			.AnyAsync(a => a.JobId == jobId && a.UserId == userId);

		if (alreadyApplied)
			throw new InvalidOperationException("You have already applied to this job.");

		var application = new JobApplication
		{
			JobId = jobId,
			UserId = userId,
			CoverLetter = request.CoverLetter.Trim(),
			CvUrl = request.CvUrl,
			Status = ApplicationStatus.Pending,
			CreatedAt = DateTime.UtcNow
		};

		_db.JobApplications.Add(application);
		await _db.SaveChangesAsync();

		return await GetDtoAsync(application.Id);
	}

	public async Task<JobApplicationDto> ChangeStatusAsync(long applicationId, long companyId, ApplicationStatus newStatus)
	{
		var application = await _db.JobApplications
			.Include(a => a.Job)
			.FirstOrDefaultAsync(a => a.Id == applicationId)
			?? throw new InvalidOperationException("Application not found.");

		// Verify this company owns the job the application is for
		if (application.Job!.CompanyId != companyId)
			throw new InvalidOperationException("You don't have permission to update this application.");

		// Validate the status transition
		var allowed = application.Status switch
		{
			ApplicationStatus.Pending => newStatus is ApplicationStatus.Reviewed or ApplicationStatus.Rejected,
			ApplicationStatus.Reviewed => newStatus is ApplicationStatus.Accepted or ApplicationStatus.Rejected,
			_ => false // Accepted and Rejected are terminal — no further changes
		};

		if (!allowed)
			throw new InvalidOperationException(
				$"Cannot change status from {application.Status} to {newStatus}.");

		// Record history before changing
		_db.ApplicationStatusHistories.Add(new ApplicationStatusHistory
		{
			ApplicationId = application.Id,
			OldStatus = application.Status,
			NewStatus = newStatus,
			ChangedAt = DateTime.UtcNow
		});

		application.Status = newStatus;
		application.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync();

		return await GetDtoAsync(application.Id);
	}

	public async Task<PagedResult<JobApplicationDto>> GetByJobAsync(long jobId, long companyId, int pageNumber, int pageSize)
	{
		// Verify the company owns this job
		var jobExists = await _db.Jobs
			.AnyAsync(j => j.Id == jobId && j.CompanyId == companyId);

		if (!jobExists)
			throw new InvalidOperationException("Job not found or you don't have permission.");

		var total = await _db.JobApplications.CountAsync(a => a.JobId == jobId);

		var items = await _db.JobApplications
			.Include(a => a.User)
			.Include(a => a.Job).ThenInclude(j => j!.Company)
			.Include(a => a.StatusHistory)
			.Where(a => a.JobId == jobId)
			.OrderByDescending(a => a.CreatedAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.Select(a => MapToDto(a))
			.ToListAsync();

		return new PagedResult<JobApplicationDto>
		{
			Items = items,
			TotalCount = total,
			PageNumber = pageNumber,
			PageSize = pageSize
		};
	}

	public async Task<PagedResult<JobApplicationDto>> GetByUserAsync(long userId, int pageNumber, int pageSize)
	{
		var total = await _db.JobApplications.CountAsync(a => a.UserId == userId);

		var items = await _db.JobApplications
			.Include(a => a.User)
			.Include(a => a.Job).ThenInclude(j => j!.Company)
			.Include(a => a.StatusHistory)
			.Where(a => a.UserId == userId)
			.OrderByDescending(a => a.CreatedAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.Select(a => MapToDto(a))
			.ToListAsync();

		return new PagedResult<JobApplicationDto>
		{
			Items = items,
			TotalCount = total,
			PageNumber = pageNumber,
			PageSize = pageSize
		};
	}

	public async Task<JobApplicationDto?> GetByIdAsync(long applicationId)
	{
		var exists = await _db.JobApplications.AnyAsync(a => a.Id == applicationId);
		if (!exists) return null;

		return await GetDtoAsync(applicationId);
	}

	private async Task<JobApplicationDto> GetDtoAsync(long applicationId)
	{
		var application = await _db.JobApplications
			.Include(a => a.User)
			.Include(a => a.Job).ThenInclude(j => j!.Company)
			.Include(a => a.StatusHistory)
			.FirstAsync(a => a.Id == applicationId);

		return MapToDto(application);
	}

	private static JobApplicationDto MapToDto(JobApplication a) => new()
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