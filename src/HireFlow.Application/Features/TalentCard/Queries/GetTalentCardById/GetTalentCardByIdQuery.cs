using HireFlow.Application.DTOs.TalentCard;
using MediatR;

namespace HireFlow.Application.Features.TalentCard.Queries.GetTalentCardById;

public record GetTalentCardByIdQuery(long TalentCardId) : IRequest<TalentCardDto>;
