using HireFlow.Application.Common.Mappings;
using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.JobApplication;
using HireFlow.Application.Interfaces;
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
		var job = await _db.Jobs.FindAsync(new object[] { request.JobId }, cancellationToken)
			?? throw new NotFoundException("Job", request.JobId);

		var companyId = _currentUser.CompanyId;

		if (job.CompanyId != companyId)
			throw new ForbiddenException("You can only view applications for your own job listings.");

		var total = await _db.JobApplications.CountAsync(a => a.JobId == request.JobId, cancellationToken);

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
