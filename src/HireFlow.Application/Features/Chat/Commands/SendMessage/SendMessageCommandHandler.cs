using HireFlow.Application.DTOs.Chat.Responses;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Chat.Commands.SendMessage;

public class SendMessageCommandHandler
	: IRequestHandler<SendMessageCommand, MessageDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;
	private readonly IChatNotificationService _chatNotification;

	public SendMessageCommandHandler(
		IAppDbContext db,
		ICurrentUser currentUser,
		IChatNotificationService chatNotification) 
	{
		_db = db;
		_currentUser = currentUser;
		_chatNotification = chatNotification;
	}

	public async Task<MessageDto> Handle(
		SendMessageCommand command, CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;

		var application = await _db.JobApplications
			.Include(a => a.Job).ThenInclude(j => j!.Company)
			.Include(x => x.User)
			.FirstOrDefaultAsync(a => a.Id == command.ApplicationId, cancellationToken)
			?? throw new NotFoundException("Application", command.ApplicationId);

		var isFreelancer = application.UserId == userId;
		var isCompany = application.Job?.Company?.UserId == userId;

		if (!isFreelancer && !isCompany)
			throw new ForbiddenException(
				"You can only send messages in conversations you are part of.");

		if (application.Status == ApplicationStatus.Withdrawn)
			throw new InvalidOperationDomainException(
				"Cannot send messages on a withdrawn application.");

		var message = new Message
		{
			ApplicationId = command.ApplicationId,
			SenderId = userId,
			Content = command.Request.Content.Trim(),
			IsRead = false,
			SentAt = DateTime.UtcNow
		};

		_db.Messages.Add(message);
		await _db.SaveChangesAsync(cancellationToken);

		var messageDto = new MessageDto
		{
			Id = message.Id,
			SenderId = message.SenderId,
			SenderName = application.User.FullName,
			SenderRole = application.User.Role.ToString(),
			Content = message.Content,
			IsRead = message.IsRead,
			SentAt = message.SentAt,
			IsOwnMessage = true
		};

		await _chatNotification.SendMessageToConversation(
			command.ApplicationId, messageDto);

		return messageDto;
	}
}
