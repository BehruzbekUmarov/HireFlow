using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Job.Commands.DeleteJob;

public class AdminDeleteJobCommandHandler
	: IRequestHandler<AdminDeleteJobCommand>
{
	private readonly IAppDbContext _db;

	public AdminDeleteJobCommandHandler(IAppDbContext db) => _db = db;

	public async Task Handle(
		AdminDeleteJobCommand command, CancellationToken cancellationToken)
	{
		var job = await _db.Jobs
			.FirstOrDefaultAsync(j => j.Id == command.JobId, cancellationToken)
			?? throw new NotFoundException("Job", command.JobId);

		job.IsDeleted = true;
		job.DeletedAt = DateTime.UtcNow;
		job.IsActive = false;

		await _db.SaveChangesAsync(cancellationToken);
	}
}
