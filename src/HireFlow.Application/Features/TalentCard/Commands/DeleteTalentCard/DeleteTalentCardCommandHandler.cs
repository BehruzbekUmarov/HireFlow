using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.TalentCard.Commands.DeleteTalentCard;

public class DeleteTalentCardCommandHandler
	: IRequestHandler<DeleteTalentCardCommand>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public DeleteTalentCardCommandHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task Handle(
		DeleteTalentCardCommand command, CancellationToken cancellationToken)
	{
		var card = await _db.TalentCards
			.FirstOrDefaultAsync(t => t.Id == command.TalentCardId, cancellationToken)
			?? throw new NotFoundException("TalentCard", command.TalentCardId);

		if (card.UserId != _currentUser.UserId)
			throw new ForbiddenException("You can only delete your own talent cards.");

		_db.TalentCards.Remove(card);
		await _db.SaveChangesAsync(cancellationToken);
	}
}
