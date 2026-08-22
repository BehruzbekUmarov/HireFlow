using MediatR;

namespace HireFlow.Application.Features.Chat.Commands.MarkMessage;

public record MarkMessagesAsReadCommand(long ApplicationId) : IRequest;
