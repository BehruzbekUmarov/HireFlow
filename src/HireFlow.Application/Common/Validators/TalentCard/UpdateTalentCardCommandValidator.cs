using FluentValidation;
using HireFlow.Application.Features.TalentCard.Commands.UpdateTalentCard;

namespace HireFlow.Application.Common.Validators.TalentCard;

public class UpdateTalentCardCommandValidator
	: AbstractValidator<UpdateTalentCardCommand>
{
	public UpdateTalentCardCommandValidator()
	{
		RuleFor(x => x.Request.Title)
			.NotEmpty().WithMessage("Title is required.")
			.MinimumLength(5).WithMessage("Title must be at least 5 characters.")
			.MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

		RuleFor(x => x.Request.Description)
			.NotEmpty().WithMessage("Description is required.")
			.MinimumLength(20).WithMessage("Description must be at least 20 characters.")
			.MaximumLength(3000).WithMessage("Description must not exceed 3000 characters.");

		RuleFor(x => x.Request.Category)
			.NotEmpty().WithMessage("Category is required.")
			.MaximumLength(100).WithMessage("Category must not exceed 100 characters.");

		RuleFor(x => x.Request.Skills)
			.NotEmpty().WithMessage("Skills are required.")
			.MaximumLength(1000).WithMessage("Skills must not exceed 1000 characters.");

		RuleFor(x => x.Request.HourlyRate)
			.GreaterThan(0).WithMessage("Hourly rate must be greater than 0.")
			.LessThan(10000).WithMessage("Hourly rate seems unrealistically high.");
	}
}
