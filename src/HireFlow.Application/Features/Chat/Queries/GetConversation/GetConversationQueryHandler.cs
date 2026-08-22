using HireFlow.Application.DTOs.Chat.Responses;
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
		GetConversationQuery query, CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;

		var application = await _db.JobApplications
			.Include(a => a.Job)
				.ThenInclude(j => j!.Company)
			.Include(a => a.User)
			.Include(a => a.Messages)
				.ThenInclude(m => m.Sender)
			.FirstOrDefaultAsync(a => a.Id == query.ApplicationId, cancellationToken)
			?? throw new NotFoundException("Application", query.ApplicationId);

		// Only freelancer who applied OR company that owns the job can view
		var isFreelancer = application.UserId == userId;
		var isCompany = application.Job?.Company?.UserId == userId;

		if (!isFreelancer && !isCompany)
			throw new ForbiddenException(
				"You can only view conversations you are part of.");

		var messages = application.Messages
			.OrderBy(m => m.SentAt)
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
			.ToList();

		return new ConversationDto
		{
			ApplicationId = application.Id,
			JobTitle = application.Job.Title,
			CompanyName = application.Job.Company.Name,
			FreelancerName = application.User!.FullName,
			Messages = messages,
			UnreadCount = messages.Count(m => !m.IsRead && m.SenderId != userId)
		};
	}
}
