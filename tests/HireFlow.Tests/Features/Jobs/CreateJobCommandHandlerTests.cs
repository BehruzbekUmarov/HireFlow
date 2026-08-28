using FluentAssertions;
using HireFlow.Application.DTOs.Job;
using HireFlow.Application.DTOs.Job.Requests;
using HireFlow.Application.Features.Job.Commands.CreateJob;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Errors;
using HireFlow.Infrastructure.Persistence;
using HireFlow.Tests.Common;
using Moq;

namespace HireFlow.Tests.Features.Jobs;

public class CreateJobCommandHandlerTests : TestBase
{
	private readonly TestAppDbContext _db;
	private readonly Mock<ICacheService> _cacheMock;
	private readonly CreateJobCommandHandler _handler;

	public CreateJobCommandHandlerTests()
	{
		_db = TestDbContextFactory.Create();
		_cacheMock = new Mock<ICacheService>();

		// Cache does nothing in tests � no Redis needed
		_cacheMock
			.Setup(c => c.RemoveByPrefixAsync(It.IsAny<string>()))
			.Returns(Task.CompletedTask);

		_handler = new CreateJobCommandHandler(
			_db,
			CurrentUserMock.Object,
			_cacheMock.Object); // ? add this
	}

	private async Task<(User user, Company company)> SeedApprovedCompanyAsync()
	{
		var user = new User
		{
			Email = "company@gmail.com",
			PasswordHash = "hash",
			FullName = "Company Owner",
			Role = Domain.Enums.UserRole.Company
		};

		var company = new Company
		{
			Name = "TechCorp",
			IsApproved = true,
			User = user
		};

		user.Company = company;
		_db.Users.Add(user);
		await _db.SaveChangesAsync();

		return (user, company);
	}

	[Fact]
	public async Task Handle_ApprovedCompany_CreatesJob()
	{
		var (_, company) = await SeedApprovedCompanyAsync();
		SetCurrentUser(userId: 1, companyId: company.Id);

		var request = new CreateJobRequest
		{
			Title = ".NET Developer",
			Description = "We need a skilled developer with experience in ASP.NET Core.",
			Category = "Backend",
			Location = "Tashkent",
			Salary = 2000
		};

		var result = await _handler.Handle(
			new CreateJobCommand(request),
			CancellationToken.None);

		result.IsSuccess.Should().BeTrue();
		result.Value.Title.Should().Be(".NET Developer");
		result.Value.Salary.Should().Be(2000);

		var savedJob = _db.Jobs.FirstOrDefault();
		savedJob.Should().NotBeNull();
		savedJob!.CompanyId.Should().Be(company.Id);
	}

	[Fact]
	public async Task Handle_ApprovedCompany_InvalidatesCacheAfterCreate()
	{
		// Bonus test � verify cache is cleared after job created
		var (_, company) = await SeedApprovedCompanyAsync();
		SetCurrentUser(userId: 1, companyId: company.Id);

		var request = new CreateJobRequest
		{
			Title = ".NET Developer",
			Description = "We need a skilled developer with experience in ASP.NET Core.",
			Category = "Backend",
			Location = "Tashkent",
			Salary = 2000
		};

		await _handler.Handle(
			new CreateJobCommand(request),
			CancellationToken.None);

		// Verify cache was invalidated
		_cacheMock.Verify(
			c => c.RemoveByPrefixAsync(It.IsAny<string>()),
			Times.Once);
	}

	[Fact]
	public async Task Handle_UnapprovedCompany_ReturnsCompanyNotApprovedError()
	{
		var user = new User
		{
			Email = "company@gmail.com",
			PasswordHash = "hash",
			FullName = "Owner",
			Role = Domain.Enums.UserRole.Company
		};

		var company = new Company
		{
			Name = "TechCorp",
			IsApproved = false,
			User = user
		};

		user.Company = company;
		_db.Users.Add(user);
		await _db.SaveChangesAsync();

		SetCurrentUser(userId: user.Id, companyId: company.Id);

		var request = new CreateJobRequest
		{
			Title = ".NET Developer",
			Description = "Looking for developer",
			Category = "Backend",
			Location = "Tashkent",
			Salary = 2000
		};

		var result = await _handler.Handle(
			new CreateJobCommand(request),
			CancellationToken.None);

		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(DomainErrors.Company.NotApproved);
	}

	[Fact]
	public async Task Handle_NoCompanyId_ReturnsCompanyNotAssociatedError()
	{
		SetCurrentUser(userId: 1, companyId: null);

		var request = new CreateJobRequest
		{
			Title = ".NET Developer",
			Description = "Looking for developer",
			Category = "Backend",
			Location = "Tashkent",
			Salary = 2000
		};

		var result = await _handler.Handle(
			new CreateJobCommand(request),
			CancellationToken.None);

		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(DomainErrors.Company.NotAssociated);
	}
}
