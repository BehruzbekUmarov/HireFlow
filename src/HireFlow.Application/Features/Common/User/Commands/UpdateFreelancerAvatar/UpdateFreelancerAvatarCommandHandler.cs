using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Common.User.Commands.UpdateFreelancerAvatar;

public class UpdateFreelancerAvatarCommandHandler
	: IRequestHandler<UpdateFreelancerAvatarCommand>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public UpdateFreelancerAvatarCommandHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}


	public async Task Handle(
		UpdateFreelancerAvatarCommand command, CancellationToken ct)
	{
		var user = await _db.Users
			.FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, ct)
			?? throw new NotFoundException("User", _currentUser.UserId);

		user.ProfilePictureUrl = command.Url;
		user.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync(ct);
	}
}