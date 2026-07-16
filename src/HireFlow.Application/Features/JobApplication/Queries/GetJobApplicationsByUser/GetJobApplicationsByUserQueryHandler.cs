using HireFlow.Application.Common.Mappings;
using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.JobApplication;
using HireFlow.Application.Interfaces;
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

		var total = await _db.JobApplications.CountAsync(a => a.UserId == userId, cancellationToken);

		var items = await _db.JobApplications
			.AsNoTracking()
			.Where(a => a.UserId == userId)
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
