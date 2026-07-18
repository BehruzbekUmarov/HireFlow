using HireFlow.Application.Common.Mappings;
using HireFlow.Application.DTOs.JobApplication;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.JobApplication.Commands.ChangeApplicationStatus;

public sealed class ChangeApplicationStatusCommandHandler : IRequestHandler<ChangeApplicationStatusCommand, JobApplicationDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public ChangeApplicationStatusCommandHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<JobApplicationDto> Handle(ChangeApplicationStatusCommand command, CancellationToken cancellationToken)
	{
		var application = await _db.JobApplications
			.Include(a => a.Job)
			.FirstOrDefaultAsync(a => a.Id == command.ApplicationId, cancellationToken)
			?? throw new NotFoundException("Application", command.ApplicationId);

		var companyId = _currentUser.CompanyId;

		if (application.Job!.CompanyId != companyId)
			throw new ForbiddenException("You can only manage applications for your own job listings.");

		var allowed = application.Status switch
		{
			ApplicationStatus.Pending => command.NewStatus is ApplicationStatus.Reviewed or ApplicationStatus.Rejected,
			ApplicationStatus.Reviewed => command.NewStatus is ApplicationStatus.Accepted or ApplicationStatus.Rejected,
			_ => false
		};

		if (!allowed)
			throw new InvalidOperationDomainException(
				$"Cannot change status from '{application.Status}' to '{command.NewStatus}'.");

		_db.ApplicationStatusHistories.Add(new ApplicationStatusHistory
		{
			ApplicationId = application.Id,
			OldStatus = application.Status,
			NewStatus = command.NewStatus,
			ChangedAt = DateTime.UtcNow
		});

		application.Status = command.NewStatus;
		application.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync(cancellationToken);

		return await _db.JobApplications
			.Where(a => a.Id == application.Id)
			.Select(JobApplicationMapping.ProjectToDto())
			.FirstAsync(cancellationToken);
	}
}
