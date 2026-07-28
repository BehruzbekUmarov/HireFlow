using HireFlow.Application.Common.Mappings;
using HireFlow.Application.DTOs.JobApplication;
using HireFlow.Application.Events;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.JobApplication.Commands.ChangeApplicationStatus;

public sealed class ChangeApplicationStatusCommandHandler : IRequestHandler<ChangeApplicationStatusCommand, JobApplicationDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;
	private readonly IPublishEndpoint _publishEndpoint;

	public ChangeApplicationStatusCommandHandler(IAppDbContext db, ICurrentUser currentUser, IPublishEndpoint publishEndpoint)
	{
		_db = db;
		_currentUser = currentUser;
		_publishEndpoint = publishEndpoint;
	}

	public async Task<JobApplicationDto> Handle(ChangeApplicationStatusCommand command, CancellationToken cancellationToken)
	{
		var application = await _db.JobApplications
			.Include(a => a.Job).ThenInclude(j => j!.Company)
			.Include(a => a.User)
			.FirstOrDefaultAsync(a => a.Id == command.ApplicationId, cancellationToken)
			?? throw new NotFoundException("Application", command.ApplicationId);

		var companyId = _currentUser.CompanyId
			?? throw new ForbiddenException("Company must be provided.");

		if (application.Job!.CompanyId != companyId)
			throw new ForbiddenException("You can only manage your own job applications.");

		var allowed = application.Status switch
		{
			ApplicationStatus.Pending => command.NewStatus is ApplicationStatus.Reviewed
										  or ApplicationStatus.Rejected,
			ApplicationStatus.Reviewed => command.NewStatus is ApplicationStatus.Accepted
										  or ApplicationStatus.Rejected,
			_ => false
		};

		if (!allowed)
			throw new InvalidOperationDomainException(
				$"Cannot change status from '{application.Status}' to '{command.NewStatus}'.");

		var oldStatus = application.Status;

		_db.ApplicationStatusHistories.Add(new ApplicationStatusHistory
		{
			ApplicationId = application.Id,
			OldStatus = oldStatus,
			NewStatus = command.NewStatus,
			ChangedAt = DateTime.UtcNow
		});

		application.Status = command.NewStatus;
		application.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync(cancellationToken);

		await _publishEndpoint.Publish(new ApplicationStatusChangedEvent
		{
			FreelancerEmail = application.User!.Email,
			FreelancerFullName = application.User.FullName,
			JobTitle = application.Job.Title,
			CompanyName = application.Job.Company!.Name,
			OldStatus = oldStatus.ToString(),
			NewStatus = command.NewStatus.ToString(),
			ChangedAt = DateTime.UtcNow
		}, cancellationToken);

		return await _db.JobApplications
			.Where(a => a.Id == application.Id)
			.Select(JobApplicationMapping.ProjectToDto())
			.FirstAsync(cancellationToken);
	}
}
