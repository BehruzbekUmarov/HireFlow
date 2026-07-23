using HireFlow.Application.Common.Mappings;
using HireFlow.Application.DTOs.JobApplication;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class SubmitApplicationCommandHandler : IRequestHandler<SubmitApplicationCommand, JobApplicationDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public SubmitApplicationCommandHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<JobApplicationDto> Handle(SubmitApplicationCommand command, CancellationToken cancellationToken)
	{
		var job = await _db.Jobs
			.AsNoTracking()
			.FirstOrDefaultAsync(j => j.Id == command.JobId, cancellationToken);

		if (job is null)
			throw new NotFoundException("job", command.JobId);

		if (!job.IsActive)
			throw new InvalidOperationDomainException("This job is no longer accepting applications.");

		var userId = _currentUser.UserId;

		var hasAlreadyApplied = await _db.JobApplications
			.AnyAsync(a => a.JobId == command.JobId && a.UserId == userId, cancellationToken);

		if (hasAlreadyApplied)
			throw new ConflictException("You have already applied to this job.");

		var application = new JobApplication
		{
			JobId = command.JobId,
			UserId = userId,
			CoverLetter = command.Request.CoverLetter.Trim(),
			CvUrl = command.Request.CvUrl,
			Status = ApplicationStatus.Pending,
			CreatedAt = DateTime.UtcNow
		};

		_db.JobApplications.Add(application);
		await _db.SaveChangesAsync(cancellationToken);

		return await _db.JobApplications
			.Where(a => a.Id == application.Id)
			.Select(JobApplicationMapping.ProjectToDto())
			.FirstAsync(cancellationToken);
	}
}