using HireFlow.Application.DTOs.TalentCard;
using HireFlow.Application.Features.TalentCard.Commands.CreateTalentCard;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.TalentCard.Queries.GetTalentCardById;

public class GetTalentCardByIdQueryHandler
	: IRequestHandler<GetTalentCardByIdQuery, TalentCardDto>
{
	private readonly IAppDbContext _db;

	public GetTalentCardByIdQueryHandler(IAppDbContext db) => _db = db;

	public async Task<TalentCardDto> Handle(
		GetTalentCardByIdQuery query, CancellationToken cancellationToken)
	{
		var card = await _db.TalentCards
			.Include(t => t.User)
			.FirstOrDefaultAsync(t => t.Id == query.TalentCardId, cancellationToken)
			?? throw new NotFoundException("TalentCard", query.TalentCardId);

		return CreateTalentCardCommandHandler.MapToDto(card, card.User!.FullName);
	}
}
