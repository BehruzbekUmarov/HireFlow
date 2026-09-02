using HireFlow.Application.DTOs.TalentCard;
using MediatR;

namespace HireFlow.Application.Features.TalentCard.Commands.CreateTalentCard;

public record CreateTalentCardCommand(
	CreateTalentCardRequest Request) : IRequest<TalentCardDto>;
