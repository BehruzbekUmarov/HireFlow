using HireFlow.Application.DTOs.Auth.Responses;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Common.Auth.Commands.RegisterCompany;

public sealed class RegisterCompanyCommandHandler
	: IRequestHandler<RegisterCompanyCommand, RegisterResponse>
{
	private readonly IAppDbContext _db;
	private readonly IPasswordHasher _passwordHasher;

	public RegisterCompanyCommandHandler(
		IAppDbContext db,
		IPasswordHasher passwordHasher)
	{
		_db = db;
		_passwordHasher = passwordHasher;
	}

	public async Task<RegisterResponse> Handle(
		RegisterCompanyCommand command,
		CancellationToken cancellationToken)
	{
		var request = command.Request;

		var email = request.Email
			.Trim()
			.ToLowerInvariant();

		var emailExists = await _db.Users
			.AnyAsync(
				u => u.Email == email,
				cancellationToken);

		if (emailExists)
		{
			throw new ConflictException(
				"Email is already registered.");
		}

		var now = DateTime.UtcNow;

		var user = new Domain.Entities.User
		{
			Email = email,
			PasswordHash = _passwordHasher.Hash(request.Password),
			FullName = request.FullName.Trim(),
			Role = UserRole.Company,
			CreatedAt = now,

			Company = new Company
			{
				Name = request.CompanyName.Trim(),
				IsApproved = false,
				CreatedAt = now
			}
		};

		_db.Users.Add(user);

		await _db.SaveChangesAsync(cancellationToken);

		return new RegisterResponse
		{
			Id = user.Id,
			Email = user.Email,
			FullName = user.FullName,
			Role = user.Role.ToString(),
			CreatedAt = user.CreatedAt,
			Message =
				"Company account created. Pending admin approval before posting jobs."
		};
	}
}