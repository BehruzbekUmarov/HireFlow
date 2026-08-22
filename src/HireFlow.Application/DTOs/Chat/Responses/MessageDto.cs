namespace HireFlow.Application.DTOs.Chat.Responses;

public class MessageDto
{
	public long Id { get; set; }
	public long SenderId { get; set; }
	public string SenderName { get; set; } = string.Empty;
	public string SenderRole { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
	public bool IsRead { get; set; }
	public DateTime SentAt { get; set; }
	public bool IsOwnMessage { get; set; }
}