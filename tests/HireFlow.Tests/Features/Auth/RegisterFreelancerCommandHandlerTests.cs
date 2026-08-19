using FluentAssertions;
using HireFlow.Application.DTOs.Auth.Requests;
using HireFlow.Application.Features.Common.Auth.Commands.RegisterFreelancer;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Exceptions;
using HireFlow.Infrastructure.Persistence;
using HireFlow.Tests.Common;
using Moq;

namespace HireFlow.Tests.Features.Auth;

public class RegisterFreelancerCommandHandlerTests : TestBase
{
	private readonly TestAppDbContext _db;
	private readonly Mock<IPasswordHasher> _passwordHasherMock;
	private readonly RegisterFreelancerCommandHandler _handler;

	// ? removed _tokenServiceMock — register doesn't use tokens

	public RegisterFreelancerCommandHandlerTests()
	{
		_db = TestDbContextFactory.Create();
		_passwordHasherMock = new Mock<IPasswordHasher>();

		_passwordHasherMock
			.Setup(h => h.Hash(It.IsAny<string>()))
			.Returns("hashed_password");

		// ? removed all _tokenServiceMock setup — not needed here

		_handler = new RegisterFreelancerCommandHandler(
			_db,
			_passwordHasherMock.Object);
	}

	[Fact]
	public async Task Handle_ValidRequest_CreatesUserAndReturnsResponse()
	{
		var request = new RegisterFreelancerRequest
		{
			Email = "ali@gmail.com",
			Password = "Test123!",
			FullName = "Ali Karimov"
		};

		var result = await _handler.Handle(
			new RegisterFreelancerCommand(request),
			CancellationToken.None);

		result.Should().NotBeNull();
		result.Email.Should().Be("ali@gmail.com");
		result.Role.Should().Be("Freelancer");

		var savedUser = _db.Users.FirstOrDefault();
		savedUser.Should().NotBeNull();
		savedUser!.Email.Should().Be("ali@gmail.com");
		savedUser.Role.Should().Be(UserRole.Freelancer);
	}

	[Fact]
	public async Task Handle_EmailNormalized_ToLowercase()
	{
		var request = new RegisterFreelancerRequest
		{
			Email = "ALI@GMAIL.COM",
			Password = "Test123!",
			FullName = "Ali"
		};

		await _handler.Handle(
			new RegisterFreelancerCommand(request),
			CancellationToken.None);

		var savedUser = _db.Users.FirstOrDefault();
		savedUser!.Email.Should().Be("ali@gmail.com");
	}

	[Fact]
	public async Task Handle_DuplicateEmail_ThrowsConflictException()
	{
		var request = new RegisterFreelancerRequest
		{
			Email = "ali@gmail.com",
			Password = "Test123!",
			FullName = "Ali"
		};

		await _handler.Handle(
			new RegisterFreelancerCommand(request),
			CancellationToken.None);

		var act = async () => await _handler.Handle(
			new RegisterFreelancerCommand(request),
			CancellationToken.None);

		await act.Should().ThrowAsync<ConflictException>()
			.WithMessage("*already registered*");
	}

	[Fact]
	public async Task Handle_ValidRequest_PasswordIsHashed()
	{
		var request = new RegisterFreelancerRequest
		{
			Email = "ali@gmail.com",
			Password = "Test123!",
			FullName = "Ali"
		};

		await _handler.Handle(
			new RegisterFreelancerCommand(request),
			CancellationToken.None);

		var savedUser = _db.Users.FirstOrDefault();
		savedUser!.PasswordHash.Should().Be("hashed_password");
		savedUser.PasswordHash.Should().NotBe("Test123!");

		_passwordHasherMock.Verify(
			h => h.Hash("Test123!"),
			Times.Once);
	}
}
