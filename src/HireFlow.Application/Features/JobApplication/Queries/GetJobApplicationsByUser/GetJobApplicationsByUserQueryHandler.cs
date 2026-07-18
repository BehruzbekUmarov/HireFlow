using HireFlow.Application.Common.Mappings;
using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.JobApplication;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.JobApplication.Queries.GetJobApplicationsByUser;

public class GetJobApplicationsByUserQueryHandler : IRequestHandler<GetJobApplicationsByUserQuery, PagedResult<JobApplicationDto>>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public GetJobApplicationsByUserQueryHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<PagedResult<JobApplicationDto>> Handle(GetJobApplicationsByUserQuery request, CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;

		if (userId is 0)
			throw new ForbiddenException("You must be logged in to view your job applications.");

		var query = _db.JobApplications
			.AsNoTracking()
			.Where(a => a.UserId == userId);

		var total = await query.CountAsync(cancellationToken);

		var items = await query
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
