using HireFlow.Application.Common.Constants;
using HireFlow.Application.Common.Mappings;
using HireFlow.Application.DTOs.Job;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Job.Commands.CreateJob;

public sealed class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, JobDetailDto>
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

	public async Task<JobDetailDto> Handle(CreateJobCommand command, CancellationToken cancellationToken)
	{
		long companyId = _currentUser.CompanyId
		?? throw new ForbiddenException("You must be associated with a company to post a job.");

		var company = await _db.Companies.FindAsync(companyId, cancellationToken)
			?? throw new NotFoundException($"Company not found {companyId}");

		if (!company.IsApproved)
			throw new InvalidOperationException("Your company must be approved before posting jobs.");

		var job = new Domain.Entities.Job
		{
			CompanyId = companyId,
			Title = command.Request.Title.Trim(),
			Description = command.Request.Description.Trim(),
			Category = command.Request.Category.Trim(),
			Location = command.Request.Location.Trim(),
			Salary = command.Request.Salary,
			IsActive = true,
			CreatedAt = DateTime.UtcNow
		};

		_db.Jobs.Add(job);
		await _db.SaveChangesAsync(cancellationToken);

		await _cache.RemoveByPrefixAsync(CacheKeys.JobSearchPrefix);

		return await _db.Jobs
			.Where(j => j.Id == job.Id)
			.Select(JobMapping.ProjectToDetailDto())
			.FirstAsync(cancellationToken);
	}
}