using HireFlow.Application.DTOs.Chat.Responses;
using HireFlow.Application.DTOs.Common;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Chat.Queries.GetConversation;

public class GetConversationQueryHandler
	: IRequestHandler<GetConversationQuery, ConversationDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public GetConversationQueryHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<ConversationDto> Handle(
		GetConversationQuery query,
		CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;

		var application = await _db.JobApplications
			.AsNoTracking()
			.Where(a => a.Id == query.ApplicationId)
			.Select(a => new
			{
				a.Id,
				JobTitle = a.Job!.Title,
				CompanyName = a.Job.Company!.Name,
				FreelancerName = a.User!.FullName,
				FreelancerId = a.UserId,
				CompanyUserId = a.Job.Company.UserId
			})
			.FirstOrDefaultAsync(cancellationToken)
			?? throw new NotFoundException(
				"Application",
				query.ApplicationId);

		var isFreelancer = application.FreelancerId == userId;
		var isCompany = application.CompanyUserId == userId;

		if (!isFreelancer && !isCompany)
		{
			throw new ForbiddenException(
				"You can only view conversations you are part of.");
		}

		var messagesQuery = _db.Messages
			.AsNoTracking()
			.Where(m => m.ApplicationId == query.ApplicationId);

		var totalMessages = await messagesQuery
			.CountAsync(cancellationToken);

		var unreadCount = await messagesQuery
			.CountAsync(
				m => !m.IsRead && m.SenderId != userId,
				cancellationToken);

		var messages = await messagesQuery
			.OrderByDescending(m => m.SentAt)
			.Skip((query.PageNumber - 1) * query.PageSize)
			.Take(query.PageSize)
			.Select(m => new MessageDto
			{
				Id = m.Id,
				SenderId = m.SenderId,
				SenderName = m.Sender!.FullName,
				SenderRole = m.Sender.Role.ToString(),
				Content = m.Content,
				IsRead = m.IsRead,
				SentAt = m.SentAt,
				IsOwnMessage = m.SenderId == userId
			})
			.ToListAsync(cancellationToken);

		messages.Reverse();

		return new ConversationDto
		{
			ApplicationId = application.Id,
			JobTitle = application.JobTitle,
			CompanyName = application.CompanyName,
			FreelancerName = application.FreelancerName,

			Messages = new PagedResult<MessageDto>
			{
				Items = messages,
				TotalCount = totalMessages,
				PageNumber = query.PageNumber,
				PageSize = query.PageSize
			},

			UnreadCount = unreadCount
		};
	}
}
