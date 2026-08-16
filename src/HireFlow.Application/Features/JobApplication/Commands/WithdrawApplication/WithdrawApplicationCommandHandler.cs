using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.JobApplication.Commands.WithdrawApplication;

public class WithdrawApplicationCommandHandler
	: IRequestHandler<WithdrawApplicationCommand>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public WithdrawApplicationCommandHandler(
		IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task Handle(
		WithdrawApplicationCommand command, CancellationToken cancellationToken)
	{
		var application = await _db.JobApplications
			.FirstOrDefaultAsync(
				a => a.Id == command.ApplicationId, cancellationToken)
			?? throw new NotFoundException("Application", command.ApplicationId);

		// Only the owner can withdraw
		if (application.UserId != _currentUser.UserId)
			throw new ForbiddenException(
				"You can only withdraw your own applications.");

		// Can only withdraw if still pending or reviewed
		if (application.Status is ApplicationStatus.Accepted
			or ApplicationStatus.Rejected
			or ApplicationStatus.Withdrawn)
			throw new InvalidOperationDomainException(
				$"Cannot withdraw an application with status '{application.Status}'.");

		application.Status = ApplicationStatus.Withdrawn;
		application.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync(cancellationToken);
	}
}
