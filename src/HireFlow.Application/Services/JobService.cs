using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.Job;
using HireFlow.Application.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Services;

public class JobService : IJobService
{
	private readonly IAppDbContext _db;

	public JobService(IAppDbContext db)
	{
		_db = db;
	}

	public async Task<JobDetailDto> CreateAsync(long companyId, CreateJobRequest request)
	{
		var company = await _db.Companies.FindAsync(companyId)
			?? throw new InvalidOperationException("Company not found.");

		if (!company.IsApproved)
			throw new InvalidOperationException("Your company must be approved before posting jobs.");

		var job = new Job
		{
			CompanyId = companyId,
			Title = request.Title.Trim(),
			Description = request.Description.Trim(),
			Category = request.Category.Trim(),
			Location = request.Location.Trim(),
			Salary = request.Salary,
			IsActive = true,
			CreatedAt = DateTime.UtcNow
		};

		_db.Jobs.Add(job);
		await _db.SaveChangesAsync();

		return await GetDetailDtoAsync(job.Id);
	}

	public async Task<JobDetailDto> UpdateAsync(long jobId, long companyId, UpdateJobRequest request)
	{
		var job = await _db.Jobs.FirstOrDefaultAsync(j => j.Id == jobId && j.CompanyId == companyId)
			?? throw new InvalidOperationException("Job not found or you don't have permission.");

		job.Title = request.Title.Trim();
		job.Description = request.Description.Trim();
		job.Category = request.Category.Trim();
		job.Location = request.Location.Trim();
		job.Salary = request.Salary;
		job.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync();

		return await GetDetailDtoAsync(job.Id);
	}

	public async Task CloseAsync(long jobId, long companyId)
	{
		var job = await _db.Jobs.FirstOrDefaultAsync(j => j.Id == jobId && j.CompanyId == companyId)
			?? throw new InvalidOperationException("Job not found or you don't have permission.");

		job.IsActive = false;
		job.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync();
	}

	public async Task<JobDetailDto?> GetByIdAsync(long jobId)
	{
		var exists = await _db.Jobs.AnyAsync(j => j.Id == jobId);
		if (!exists) return null;

		return await GetDetailDtoAsync(jobId);
	}

	public async Task<PagedResult<JobSummaryDto>> SearchAsync(JobFilterRequest filter)
	{
		var query = _db.Jobs
			.Include(j => j.Company)
			.Where(j => j.IsActive)
			.AsQueryable();

		// Apply filters only when provided
		if (!string.IsNullOrWhiteSpace(filter.Keyword))
			query = query.Where(j =>
				j.Title.Contains(filter.Keyword) ||
				j.Description.Contains(filter.Keyword));

		if (!string.IsNullOrWhiteSpace(filter.Category))
			query = query.Where(j => j.Category == filter.Category);

		if (!string.IsNullOrWhiteSpace(filter.Location))
			query = query.Where(j => j.Location.Contains(filter.Location));

		if (filter.MinSalary.HasValue)
			query = query.Where(j => j.Salary >= filter.MinSalary);

		if (filter.MaxSalary.HasValue)
			query = query.Where(j => j.Salary <= filter.MaxSalary);

		// Sorting
		query = filter.SortBy switch
		{
			"Salary" => filter.SortOrder == "asc"
							? query.OrderBy(j => j.Salary)
							: query.OrderByDescending(j => j.Salary),
			"Title" => filter.SortOrder == "asc"
							? query.OrderBy(j => j.Title)
							: query.OrderByDescending(j => j.Title),
			_ => query.OrderByDescending(j => j.CreatedAt) // default: newest first
		};

		var total = await query.CountAsync();

		var items = await query
			.Skip((filter.PageNumber - 1) * filter.PageSize)
			.Take(filter.PageSize)
			.Select(j => new JobSummaryDto
			{
				Id = j.Id,
				Title = j.Title,
				CompanyName = j.Company!.Name,
				Category = j.Category,
				Location = j.Location,
				Salary = j.Salary,
				IsActive = j.IsActive,
				CreatedAt = j.CreatedAt,
				ApplicationCount = j.JobApplications.Count
			})
			.ToListAsync();

		return new PagedResult<JobSummaryDto>
		{
			Items = items,
			TotalCount = total,
			PageNumber = filter.PageNumber,
			PageSize = filter.PageSize
		};
	}

	public async Task<PagedResult<JobSummaryDto>> GetByCompanyAsync(long companyId, int pageNumber, int pageSize)
	{
		var total = await _db.Jobs.CountAsync(j => j.CompanyId == companyId);

		var items = await _db.Jobs
			.Include(j => j.Company)
			.Where(j => j.CompanyId == companyId)
			.OrderByDescending(j => j.CreatedAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.Select(j => new JobSummaryDto
			{
				Id = j.Id,
				Title = j.Title,
				CompanyName = j.Company!.Name,
				Category = j.Category,
				Location = j.Location,
				Salary = j.Salary,
				IsActive = j.IsActive,
				CreatedAt = j.CreatedAt,
				ApplicationCount = j.JobApplications.Count
			})
			.ToListAsync();

		return new PagedResult<JobSummaryDto>
		{
			Items = items,
			TotalCount = total,
			PageNumber = pageNumber,
			PageSize = pageSize
		};
	}

	// Private helper — builds the full detail DTO in one query
	private async Task<JobDetailDto> GetDetailDtoAsync(long jobId)
	{
		return await _db.Jobs
			.Include(j => j.Company)
			.Where(j => j.Id == jobId)
			.Select(j => new JobDetailDto
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
			})
			.FirstAsync();
	}
}
