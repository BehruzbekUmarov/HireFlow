using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Cv.Commands.DeleteCv;

public sealed class DeleteCvCommandHandler
	: IRequestHandler<DeleteCvCommand>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public DeleteCvCommandHandler(
		IAppDbContext db,
		ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task Handle(
		DeleteCvCommand command,
		CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;

		var cv = await _db.FreelancerCvs
			.FirstOrDefaultAsync(
				c => c.Id == command.CvId,
				cancellationToken)
			?? throw new NotFoundException(
				"CV",
				command.CvId);

		if (cv.UserId != userId)
		{
			throw new ForbiddenException(
				"You can only delete your own CVs.");
		}

		var usedInActiveApplications = await _db.JobApplications
			.AnyAsync(
				a =>
					a.CvId == cv.Id &&
					(a.Status == ApplicationStatus.Pending ||
					 a.Status == ApplicationStatus.Reviewed),
				cancellationToken);

		if (usedInActiveApplications)
		{
			throw new InvalidOperationDomainException(
				"Cannot delete a CV that is used in active applications.");
		}

		_db.FreelancerCvs.Remove(cv);

		await _db.SaveChangesAsync(cancellationToken);
	}
}