using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.JobApplication;
using HireFlow.Application.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Exceptions;
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
			?? throw new NotFoundException("job", jobId);

		if (!job.IsActive)
			throw new InvalidOperationDomainException("This job is no longer accepting applications.");

		var alreadyApplied = await _db.JobApplications
			.AnyAsync(a => a.JobId == jobId && a.UserId == userId);

		if (alreadyApplied)
			throw new ConflictException("You have already applied to this job.");

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

	public async Task<JobApplicationDto> ChangeStatusAsync(
		long applicationId, long companyId, ApplicationStatus newStatus)
	{
		var application = await _db.JobApplications
			.Include(a => a.Job)
			.FirstOrDefaultAsync(a => a.Id == applicationId)
			?? throw new NotFoundException("Application", applicationId);

		if (application.Job!.CompanyId != companyId)
			throw new ForbiddenException("You can only manage applications for your own job listings.");

		var allowed = application.Status switch
		{
			ApplicationStatus.Pending => newStatus is ApplicationStatus.Reviewed or ApplicationStatus.Rejected,
			ApplicationStatus.Reviewed => newStatus is ApplicationStatus.Accepted or ApplicationStatus.Rejected,
			_ => false
		};

		if (!allowed)
			throw new InvalidOperationDomainException(
				$"Cannot change status from '{application.Status}' to '{newStatus}'.");

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
		var job = await _db.Jobs.FindAsync(jobId)
			?? throw new NotFoundException("Job", jobId);

		if (job.CompanyId != companyId)
			throw new ForbiddenException("You can only view applications for your own job listings.");

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