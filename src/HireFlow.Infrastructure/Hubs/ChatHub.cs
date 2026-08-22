using HireFlow.Application.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace HireFlow.Infrastructure.Hubs;

[Authorize]
public class ChatHub : Hub
{
	public async Task JoinConversation(long applicationId)
		=> await Groups.AddToGroupAsync(
			   Context.ConnectionId,
			   ChatConstants.GetGroupName(applicationId));

	public async Task LeaveConversation(long applicationId)
		=> await Groups.RemoveFromGroupAsync(
			   Context.ConnectionId,
			   ChatConstants.GetGroupName(applicationId));
}