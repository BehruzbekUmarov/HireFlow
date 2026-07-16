using HireFlow.Application.Common.Mappings;
using HireFlow.Application.DTOs.Job;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Job.Queries.GetJobById;

public class GetJobByIdQueryHandler : IRequestHandler<GetJobByIdQuery, JobDetailDto?>
{
	private readonly IAppDbContext _db;

	public GetJobByIdQueryHandler(IAppDbContext db)
	{
		_db = db;
	}

	public async Task<JobDetailDto?> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
	{
		return await _db.Jobs
			.AsNoTracking()
			.Where(j => j.Id == request.JobId)
			.Select(JobMapping.ProjectToDetailDto())
			.FirstOrDefaultAsync(cancellationToken);
	}
}
