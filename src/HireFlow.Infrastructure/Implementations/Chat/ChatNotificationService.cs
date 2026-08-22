using HireFlow.Application.Common.Constants;
using HireFlow.Application.DTOs.Chat.Responses;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Infrastructure.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace HireFlow.Infrastructure.Implementations.Chat;

public class ChatNotificationService : IChatNotificationService
{
	private readonly IHubContext<ChatHub> _hubContext;

	public ChatNotificationService(IHubContext<ChatHub> hubContext)
	{
		_hubContext = hubContext;
	}

	public async Task SendMessageToConversation(
		long applicationId, MessageDto message)
	{
		await _hubContext.Clients
			.Group(ChatConstants.GetGroupName(applicationId))
			.SendAsync("NewMessage", message);
	}
}