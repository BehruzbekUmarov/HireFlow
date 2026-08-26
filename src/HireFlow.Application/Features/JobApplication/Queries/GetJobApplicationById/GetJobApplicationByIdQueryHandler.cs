using HireFlow.Application.Common.Mappings;
using HireFlow.Application.DTOs.JobApplication.Responses;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.JobApplication.Queries.GetJobApplicationById;

public class GetJobApplicationByIdQueryHandler : IRequestHandler<GetJobApplicationByIdQuery, JobApplicationDto?>
{
	private readonly IAppDbContext _db;

	public GetJobApplicationByIdQueryHandler(IAppDbContext db)
	{
		_db = db;
	}

	public async Task<JobApplicationDto?> Handle(GetJobApplicationByIdQuery request, CancellationToken cancellationToken)
	{
		return await _db.JobApplications
			.AsNoTracking()
			.Where(a => a.Id == request.ApplicationId)
			.Select(JobApplicationMapping.ProjectToDto())
			.FirstOrDefaultAsync(cancellationToken);
	}
}
