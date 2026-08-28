namespace HireFlow.Domain.Entities;

public class Message
{
	public long Id { get; set; }
	public long ApplicationId { get; set; }
	public long? SenderId { get; set; }
	public string Content { get; set; } = string.Empty;
	public bool IsRead { get; set; } = false;
	public DateTime SentAt { get; set; } = DateTime.UtcNow;

	public JobApplication? Application { get; set; }
	public User? Sender { get; set; }
}
