using HireFlow.Application.DTOs.Chat.Responses;
using MassTransit;

namespace HireFlow.Application.Services.Interfaces;

public interface IChatNotificationService
{
	Task SendMessageToConversation(long applicationId, MessageDto message);
}