using HireFlow.Application.DTOs.TalentCard;
using MediatR;

namespace HireFlow.Application.Features.TalentCard.Queries.GetMyTalentCards;

public record GetMyTalentCardsQuery : IRequest<List<TalentCardDto>>;
