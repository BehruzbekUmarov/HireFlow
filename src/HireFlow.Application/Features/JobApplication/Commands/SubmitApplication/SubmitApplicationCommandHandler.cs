using HireFlow.Application.Common.Mappings;
using HireFlow.Application.DTOs.JobApplication;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class SubmitApplicationCommandHandler
	: IRequestHandler<SubmitApplicationCommand, JobApplicationDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public SubmitApplicationCommandHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<JobApplicationDto> Handle(
		SubmitApplicationCommand command, CancellationToken cancellationToken)
	{
		var job = await _db.Jobs
			.AsNoTracking()
			.FirstOrDefaultAsync(j => j.Id == command.JobId, cancellationToken)
			?? throw new NotFoundException("Job", command.JobId);

		if (!job.IsActive)
			throw new InvalidOperationDomainException(
				"This job is no longer accepting applications.");

		var userId = _currentUser.UserId;

		var hasAlreadyApplied = await _db.JobApplications
			.AnyAsync(a => a.JobId == command.JobId
						&& a.UserId == userId, cancellationToken);

		if (hasAlreadyApplied)
			throw new ConflictException("You have already applied to this job.");

		// Resolve which CV to use
		long? cvId = null;

		if (command.Request.CvId.HasValue)
		{
			// Freelancer picked a specific CV — verify it belongs to them
			var cv = await _db.FreelancerCvs
				.FirstOrDefaultAsync(c => c.Id == command.Request.CvId
									   && c.UserId == userId, cancellationToken)
				?? throw new NotFoundException("CV", command.Request.CvId.Value);

			cvId = cv.Id;
		}
		else
		{
			// No CV picked — use default CV if exists
			var defaultCv = await _db.FreelancerCvs
				.FirstOrDefaultAsync(c => c.UserId == userId
									   && c.IsDefault, cancellationToken);

			cvId = defaultCv?.Id; // null if no default set — that's fine
		}

		var application = new JobApplication
		{
			JobId = command.JobId,
			UserId = userId,
			CoverLetter = command.Request.CoverLetter.Trim(),
			CvId = cvId,      // ← reference to FreelancerCv
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