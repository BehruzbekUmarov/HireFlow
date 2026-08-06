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

		RuleFor(x => x.Request.CvUrl)
			.MaximumLength(500).WithMessage("CV URL must not exceed 500 characters.")
			.Must(url => url == null || Uri.TryCreate(url, UriKind.Absolute, out _))
			.WithMessage("CV URL must be a valid URL.")
			.When(x => x.Request.CvUrl != null);
	}
}