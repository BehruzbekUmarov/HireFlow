using HireFlow.Domain.Enums;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Admin.Commands.DeleteUser;

public class AdminDeleteUserCommandHandler : IRequestHandler<AdminDeleteUserCommand>
{
	private readonly IAppDbContext _db;

	public AdminDeleteUserCommandHandler(IAppDbContext db) => _db = db;

	public async Task Handle(
		AdminDeleteUserCommand command, CancellationToken cancellationToken)
	{
		var user = await _db.Users
			.FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken)
			?? throw new NotFoundException("User", command.UserId);

		if (user.Role == UserRole.Admin)
			throw new ForbiddenException("Admin accounts cannot be deleted.");

		user.IsDeleted = true;
		user.DeletedAt = DateTime.UtcNow;
		user.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync(cancellationToken);
	}
}
