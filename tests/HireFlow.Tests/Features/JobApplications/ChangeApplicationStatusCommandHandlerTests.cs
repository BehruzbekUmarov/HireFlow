using FluentAssertions;
using HireFlow.Application.Events;
using HireFlow.Application.Features.JobApplication.Commands.ChangeApplicationStatus;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Exceptions;
using HireFlow.Infrastructure.Persistence;
using HireFlow.Tests.Common;
using MassTransit;
using Moq;

namespace HireFlow.Tests.Features.JobApplications;

public class ChangeApplicationStatusCommandHandlerTests : TestBase
{
	private readonly TestAppDbContext _db;
	private readonly Mock<IPublishEndpoint> _publishEndpointMock;
	private readonly ChangeApplicationStatusCommandHandler _handler;

	public ChangeApplicationStatusCommandHandlerTests()
	{
		_db = TestDbContextFactory.Create();
		_publishEndpointMock = new Mock<IPublishEndpoint>(); // ← add this

		// Setup — Publish does nothing in tests (we don't want real RabbitMQ)
		_publishEndpointMock
			.Setup(p => p.Publish(
				It.IsAny<ApplicationStatusChangedEvent>(),
				It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		_handler = new ChangeApplicationStatusCommandHandler(
			_db,
			CurrentUserMock.Object,
			_publishEndpointMock.Object); // ← add this
	}

	private async Task<(Company company, Job job, JobApplication application)>
		SeedDataAsync()
	{
		var companyUser = new User
		{
			Email = "company@test.com",
			PasswordHash = "hash",
			FullName = "Company"
		};

		var company = new Company
		{
			Name = "TechCorp",
			IsApproved = true,
			User = companyUser
		};

		companyUser.Company = company;

		var freelancerUser = new User
		{
			Email = "freelancer@test.com",
			PasswordHash = "hash",
			FullName = "Freelancer"
		};

		var job = new Job
		{
			Title = ".NET Developer",
			Description = "Description",
			Category = "Backend",
			Location = "Tashkent",
			Salary = 2000,
			IsActive = true,
			Company = company
		};

		var application = new JobApplication
		{
			Job = job,
			User = freelancerUser,
			CoverLetter = "My cover letter",
			Status = ApplicationStatus.Pending
		};

		_db.Users.AddRange(companyUser, freelancerUser);
		_db.Jobs.Add(job);
		_db.JobApplications.Add(application);
		await _db.SaveChangesAsync();

		return (company, job, application);
	}

	[Fact]
	public async Task Handle_PendingToReviewed_ChangesStatus()
	{
		// Arrange
		var (company, _, application) = await SeedDataAsync();
		SetCurrentUser(userId: 1, companyId: company.Id);

		// Act
		await _handler.Handle(
			new ChangeApplicationStatusCommand(application.Id, ApplicationStatus.Reviewed),
			CancellationToken.None);

		// Assert
		var updated = await _db.JobApplications.FindAsync(application.Id);
		updated!.Status.Should().Be(ApplicationStatus.Reviewed);
	}

	[Fact]
	public async Task Handle_AcceptedToPending_ThrowsInvalidOperation()
	{
		// Arrange
		var (company, _, application) = await SeedDataAsync();
		SetCurrentUser(userId: 1, companyId: company.Id);

		// Move to Accepted first
		application.Status = ApplicationStatus.Accepted;
		await _db.SaveChangesAsync();

		// Act & Assert — can't go back to Pending from Accepted
		var act = async () => await _handler.Handle(
			new ChangeApplicationStatusCommand(application.Id, ApplicationStatus.Pending),
			CancellationToken.None);

		await act.Should().ThrowAsync<InvalidOperationDomainException>();
	}

	[Fact]
	public async Task Handle_WrongCompany_ThrowsForbiddenException()
	{
		// Arrange
		var (_, _, application) = await SeedDataAsync();

		// Different company trying to change status
		SetCurrentUser(userId: 99, companyId: 99);

		// Act & Assert
		var act = async () => await _handler.Handle(
			new ChangeApplicationStatusCommand(application.Id, ApplicationStatus.Reviewed),
			CancellationToken.None);

		await act.Should().ThrowAsync<ForbiddenException>();
	}

	[Fact]
	public async Task Handle_StatusChange_AddsToHistory()
	{
		// Arrange
		var (company, _, application) = await SeedDataAsync();
		SetCurrentUser(userId: 1, companyId: company.Id);

		// Act
		await _handler.Handle(
			new ChangeApplicationStatusCommand(application.Id, ApplicationStatus.Reviewed),
			CancellationToken.None);

		// Assert — history should be recorded
		var history = _db.ApplicationStatusHistories
			.Where(h => h.ApplicationId == application.Id)
			.ToList();

		history.Should().HaveCount(1);
		history[0].OldStatus.Should().Be(ApplicationStatus.Pending);
		history[0].NewStatus.Should().Be(ApplicationStatus.Reviewed);
	}
}
