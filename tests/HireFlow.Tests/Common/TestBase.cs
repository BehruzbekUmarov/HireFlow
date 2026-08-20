	using HireFlow.Application.Services.Interfaces;
	using Moq;

	namespace HireFlow.Tests.Common;

	public abstract class TestBase
	{
		protected Mock<ICurrentUser> CurrentUserMock { get; } = new();

		protected void SetCurrentUser(long userId, long? companyId = null)
		{
			CurrentUserMock.Setup(u => u.UserId).Returns(userId);
			CurrentUserMock.Setup(u => u.CompanyId).Returns(companyId);
		}
	}