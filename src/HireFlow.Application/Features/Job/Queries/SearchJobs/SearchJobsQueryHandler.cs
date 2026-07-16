using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.Job;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Job.Queries.SearchJobs;

public class SearchJobsQueryHandler : IRequestHandler<SearchJobsQuery, PagedResult<JobSummaryDto>>
{
	private readonly IAppDbContext _db;

	public SearchJobsQueryHandler(IAppDbContext db)
	{
		_db = db;
	}

	public async Task<PagedResult<JobSummaryDto>> Handle(SearchJobsQuery query, CancellationToken cancellationToken)
	{
		var filter = query.Filter;

		var dbQuery = _db.Jobs
			.AsNoTracking()
			.Where(j => j.IsActive)
			.AsQueryable();

		if (!string.IsNullOrWhiteSpace(filter.Keyword))
			dbQuery = dbQuery.Where(j =>
				j.Title.Contains(filter.Keyword) ||
				j.Description.Contains(filter.Keyword));

		if (!string.IsNullOrWhiteSpace(filter.Category))
			dbQuery = dbQuery.Where(j => j.Category == filter.Category);

		if (!string.IsNullOrWhiteSpace(filter.Location))
			dbQuery = dbQuery.Where(j => j.Location.Contains(filter.Location));

		if (filter.MinSalary.HasValue)
			dbQuery = dbQuery.Where(j => j.Salary >= filter.MinSalary);

		if (filter.MaxSalary.HasValue)
			dbQuery = dbQuery.Where(j => j.Salary <= filter.MaxSalary);

		dbQuery = filter.SortBy switch
		{
			"Salary" => filter.SortOrder == "asc"
				? dbQuery.OrderBy(j => j.Salary)
				: dbQuery.OrderByDescending(j => j.Salary),
			"Title" => filter.SortOrder == "asc"
				? dbQuery.OrderBy(j => j.Title)
				: dbQuery.OrderByDescending(j => j.Title),
			_ => dbQuery.OrderByDescending(j => j.CreatedAt)
		};

		var total = await dbQuery.CountAsync(cancellationToken);
		var items = await dbQuery
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
			.ToListAsync(cancellationToken);

		return new PagedResult<JobSummaryDto>
		{
			Items = items,
			TotalCount = total,
			PageNumber = filter.PageNumber,
			PageSize = filter.PageSize
		};
	}
}