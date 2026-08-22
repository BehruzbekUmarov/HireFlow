namespace HireFlow.Application.Common.Constants;

public static class ChatConstants
{
	public static string GetGroupName(long applicationId)
		=> $"application_{applicationId}";
}
