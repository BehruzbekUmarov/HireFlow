using HireFlow.Application.Common.Mappings;
using HireFlow.Application.DTOs.Job;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Job.UpdateJob;

public class UpdateJobCommandHandler : IRequestHandler<UpdateJobCommand, JobDetailDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public UpdateJobCommandHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<JobDetailDto> Handle(UpdateJobCommand command, CancellationToken cancellationToken)
	{
		var job = await _db.Jobs.FirstOrDefaultAsync(j => j.Id == command.JobId, cancellationToken)
			?? throw new NotFoundException("Job", command.JobId);

		var companyId = _currentUser.CompanyId
			?? throw new ForbiddenException("Company must be provided to update post !");

		if (job.CompanyId != companyId)
			throw new ForbiddenException("You can only edit your own job listings.");

		job.Title = command.Request.Title.Trim();
		job.Description = command.Request.Description.Trim();
		job.Category = command.Request.Category.Trim();
		job.Location = command.Request.Location.Trim();
		job.Salary = command.Request.Salary;
		job.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync(cancellationToken);

		return await _db.Jobs
			.Where(j => j.Id == job.Id)
			.Select(JobMapping.ProjectToDetailDto())
			.FirstAsync(cancellationToken);
	}
}