using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.TalentCard;
using MediatR;

namespace HireFlow.Application.Features.TalentCard.Queries.SearchTalentCards;

public record SearchTalentCardsQuery(
	TalentCardFilterRequest Filter) : IRequest<PagedResult<TalentCardDto>>;