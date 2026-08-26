using HireFlow.Application.DTOs.Common;

namespace HireFlow.Application.DTOs.Chat.Responses;

public class ConversationDto
{
	public long ApplicationId { get; set; }
	public string JobTitle { get; set; } = string.Empty;
	public string CompanyName { get; set; } = string.Empty;
	public string FreelancerName { get; set; } = string.Empty;
	public PagedResult<MessageDto>? Messages { get; set; }
	public int UnreadCount { get; set; }
}
