using HireFlow.Application.Common.Mappings;
using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.JobApplication.Responses;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.JobApplication.Queries.GetJobApplicationByJob;

public class GetJobApplicationsByJobQueryHandler : IRequestHandler<GetJobApplicationsByJobQuery, PagedResult<JobApplicationDto>>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public GetJobApplicationsByJobQueryHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<PagedResult<JobApplicationDto>> Handle(GetJobApplicationsByJobQuery request, CancellationToken cancellationToken)
	{
		var companyId = _currentUser.CompanyId
			?? throw new ForbiddenException("You must be logged in as a company to view job applications.");

		var job = await _db.Jobs.FindAsync(request.JobId, cancellationToken)
			?? throw new NotFoundException("Job", request.JobId);

		if (job.CompanyId != companyId)
			throw new ForbiddenException("You can only view applications for your own job listings.");

		var query = _db.JobApplications
			.AsNoTracking()
			.Where(a => a.JobId == request.JobId);

		var total = await query.CountAsync(cancellationToken);

		var items = await _db.JobApplications
			.AsNoTracking()
			.Where(a => a.JobId == request.JobId)
			.OrderByDescending(a => a.CreatedAt)
			.Skip((request.PageNumber - 1) * request.PageSize)
			.Take(request.PageSize)
			.Select(JobApplicationMapping.ProjectToDto())
			.ToListAsync(cancellationToken);

		return new PagedResult<JobApplicationDto>
		{
			Items = items,
			TotalCount = total,
			PageNumber = request.PageNumber,
			PageSize = request.PageSize
		};
	}
}
