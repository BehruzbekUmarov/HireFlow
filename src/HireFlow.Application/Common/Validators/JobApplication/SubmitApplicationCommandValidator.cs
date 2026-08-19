using FluentValidation;

namespace HireFlow.Application.Common.Validators.JobApplication;

public class SubmitApplicationCommandValidator : AbstractValidator<SubmitApplicationCommand>
{
	public SubmitApplicationCommandValidator()
	{
		RuleFor(x => x.Request.CoverLetter)
			.NotEmpty().WithMessage("Cover letter is required.")
			.MinimumLength(50).WithMessage("Cover letter must be at least 50 characters.")
			.MaximumLength(3000).WithMessage("Cover letter must not exceed 3000 characters.");
	}
}