using HireFlow.Application.DTOs.TalentCard;
using HireFlow.Application.Features.TalentCard.Commands.CreateTalentCard;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.TalentCard.Commands.UpdateTalentCard;

public class UpdateTalentCardCommandHandler
	: IRequestHandler<UpdateTalentCardCommand, TalentCardDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public UpdateTalentCardCommandHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<TalentCardDto> Handle(
		UpdateTalentCardCommand command, CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;

		var card = await _db.TalentCards
			.Include(t => t.User)
			.FirstOrDefaultAsync(t => t.Id == command.TalentCardId, cancellationToken)
			?? throw new NotFoundException("TalentCard", command.TalentCardId);

		if (card.UserId != userId)
			throw new ForbiddenException("You can only edit your own talent cards.");

		var req = command.Request;
		card.Title = req.Title.Trim();
		card.Description = req.Description.Trim();
		card.Category = req.Category.Trim();
		card.Skills = req.Skills.Trim();
		card.HourlyRate = req.HourlyRate;
		card.IsActive = req.IsActive;
		card.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync(cancellationToken);

		return CreateTalentCardCommandHandler.MapToDto(card, card.User!.FullName);
	}
}
