using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Job.Commands.CloseJob;

public class CloseJobCommandHandler : IRequestHandler<CloseJobCommand>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public CloseJobCommandHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task Handle(CloseJobCommand command, CancellationToken cancellationToken)
	{
		var job = await _db.Jobs.FirstOrDefaultAsync(j => j.Id == command.JobId, cancellationToken)
			?? throw new NotFoundException("Job", command.JobId);

		var companyId = _currentUser.CompanyId;

		if (job.CompanyId != companyId)
			throw new ForbiddenException("You can only close your own job listings.");

		job.IsActive = false;
		job.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync(cancellationToken);
	}
}