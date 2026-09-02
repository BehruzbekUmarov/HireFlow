using MediatR;

namespace HireFlow.Application.Features.TalentCard.Commands.DeleteTalentCard;

public record DeleteTalentCardCommand(long TalentCardId) : IRequest;
