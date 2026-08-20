using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Cv.Commands.SetDefaultCv;

public class SetDefaultCvCommandHandler : IRequestHandler<SetDefaultCvCommand>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public SetDefaultCvCommandHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task Handle(
		SetDefaultCvCommand command, CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;

		var cv = await _db.FreelancerCvs
			.FirstOrDefaultAsync(c => c.Id == command.CvId, cancellationToken)
			?? throw new NotFoundException("CV", command.CvId);

		if (cv.UserId != userId)
			throw new ForbiddenException("You can only update your own CVs.");

		// Remove default from all others
		var others = await _db.FreelancerCvs
			.Where(c => c.UserId == userId && c.IsDefault)
			.ToListAsync(cancellationToken);

		foreach (var other in others)
			other.IsDefault = false;

		cv.IsDefault = true;
		cv.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync(cancellationToken);
	}
}
