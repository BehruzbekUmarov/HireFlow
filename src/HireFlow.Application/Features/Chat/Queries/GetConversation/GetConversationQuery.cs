using HireFlow.Application.DTOs.Chat.Responses;
using MediatR;

namespace HireFlow.Application.Features.Chat.Queries.GetConversation;

public record GetConversationQuery(long ApplicationId) : IRequest<ConversationDto>;
