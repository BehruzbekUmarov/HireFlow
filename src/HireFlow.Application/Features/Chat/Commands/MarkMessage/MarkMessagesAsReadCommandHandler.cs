using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Chat.Commands.MarkMessage;

public class MarkMessagesAsReadCommandHandler
	: IRequestHandler<MarkMessagesAsReadCommand>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public MarkMessagesAsReadCommandHandler(
		IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task Handle(
		MarkMessagesAsReadCommand command, CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;

		var unread = await _db.Messages
			.Where(m => m.ApplicationId == command.ApplicationId
					 && m.SenderId != userId
					 && !m.IsRead)
			.ToListAsync(cancellationToken);

		foreach (var message in unread)
			message.IsRead = true;

		await _db.SaveChangesAsync(cancellationToken);
	}
}
