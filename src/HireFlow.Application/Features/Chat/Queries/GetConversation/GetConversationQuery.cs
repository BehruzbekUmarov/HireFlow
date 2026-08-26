using HireFlow.Application.DTOs.Chat.Responses;
using MediatR;

namespace HireFlow.Application.Features.Chat.Queries.GetConversation;

public record GetConversationQuery(
	long ApplicationId,
	int PageNumber = 1,
	int PageSize = 50) : IRequest<ConversationDto>;
