using HireFlow.Application.DTOs.Chat.Requests;
using HireFlow.Application.DTOs.Chat.Responses;
using MediatR;

namespace HireFlow.Application.Features.Chat.Commands.SendMessage;

public record SendMessageCommand(
	long ApplicationId,
	SendMessageRequest Request) : IRequest<MessageDto>;
