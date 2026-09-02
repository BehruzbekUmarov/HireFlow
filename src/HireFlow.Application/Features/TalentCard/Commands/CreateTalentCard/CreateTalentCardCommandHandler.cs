using HireFlow.Application.DTOs.TalentCard;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.TalentCard.Commands.CreateTalentCard;

public class CreateTalentCardCommandHandler
	: IRequestHandler<CreateTalentCardCommand, TalentCardDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public CreateTalentCardCommandHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<TalentCardDto> Handle(
		CreateTalentCardCommand command, CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;

		var user = await _db.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
			?? throw new NotFoundException("User", userId);

		var talentCard = new Domain.Entities.TalentCard
		{
			UserId = userId,
			Title = command.Request.Title.Trim(),
			Description = command.Request.Description.Trim(),
			Category = command.Request.Category.Trim(),
			Skills = command.Request.Skills.Trim(),
			HourlyRate = command.Request.HourlyRate,
			IsActive = true,
			CreatedAt = DateTime.UtcNow
		};

		_db.TalentCards.Add(talentCard);
		await _db.SaveChangesAsync(cancellationToken);

		return MapToDto(talentCard, user.FullName);
	}

	public static TalentCardDto MapToDto(Domain.Entities.TalentCard card, string freelancerName) => new()
	{
		Id = card.Id,
		UserId = card.UserId,
		FreelancerName = freelancerName,
		Title = card.Title,
		Description = card.Description,
		Category = card.Category,
		Skills = card.Skills,
		HourlyRate = card.HourlyRate,
		IsActive = card.IsActive,
		CreatedAt = card.CreatedAt,
		UpdatedAt = card.UpdatedAt
	};
}
