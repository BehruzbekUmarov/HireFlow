using HireFlow.Application.Common.Constants;
using HireFlow.Application.Common.Mappings;
using HireFlow.Application.DTOs.Job.Responses;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Common;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Errors;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Job.Commands.CreateJob;

public sealed class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, Result<JobDetailDto>>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;
	private readonly ICacheService _cache;

	public CreateJobCommandHandler(
		IAppDbContext db,
		ICurrentUser currentUser,
		ICacheService cache)
	{
		_db = db;
		_currentUser = currentUser;
		_cache = cache;
	}

	public async Task<Result<JobDetailDto>> Handle(CreateJobCommand command, CancellationToken cancellationToken)
	{
		if (_currentUser.CompanyId is not { } companyId)
			return Result.Failure<JobDetailDto>(DomainErrors.Company.NotAssociated);

		var company = await _db.Companies.FindAsync(companyId, cancellationToken);
		if (company is null)
			return Result.Failure<JobDetailDto>(DomainErrors.Company.NotFound(companyId));

		if (!company.IsApproved)
			return Result.Failure<JobDetailDto>(DomainErrors.Company.NotApproved);

		var job = Domain.Entities.Job.Create(
			company,
			command.Request.Title,
			command.Request.Description,
			command.Request.Category,
			command.Request.Location,
			command.Request.Salary);

		_db.Jobs.Add(job);
		await _db.SaveChangesAsync(cancellationToken);

		await _cache.RemoveByPrefixAsync(CacheKeys.JobSearchPrefix);

		return await _db.Jobs
			.Where(j => j.Id == job.Id)
			.Select(JobMapping.ProjectToDetailDto())
			.FirstAsync(cancellationToken);
	}
}