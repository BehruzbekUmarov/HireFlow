using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.Job.Responses;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Job.Queries.GetJobsByCompany;

public sealed class GetJobsByCompanyQueryHandler : IRequestHandler<GetJobsByCompanyQuery, PagedResult<JobSummaryDto>>
{
	private readonly IAppDbContext _db;

	public GetJobsByCompanyQueryHandler(IAppDbContext db)
	{
		_db = db;
	}

	public async Task<PagedResult<JobSummaryDto>> Handle(GetJobsByCompanyQuery request, CancellationToken cancellationToken)
	{
		var query = _db.Jobs
		.AsNoTracking()
		.Where(j => j.CompanyId == request.CompanyId);

		var total = await query.CountAsync(cancellationToken);

		var items = await _db.Jobs
			.AsNoTracking()
			.Where(j => j.CompanyId == request.CompanyId)
			.OrderByDescending(j => j.CreatedAt)
			.Skip((request.PageNumber - 1) * request.PageSize)
			.Take(request.PageSize)
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
			PageNumber = request.PageNumber,
			PageSize = request.PageSize
		};
	}
}