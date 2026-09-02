using HireFlow.Application.DTOs.TalentCard;
using MediatR;

namespace HireFlow.Application.Features.TalentCard.Commands.UpdateTalentCard;

public record UpdateTalentCardCommand(
	long TalentCardId,
	UpdateTalentCardRequest Request) : IRequest<TalentCardDto>;