using FluentValidation;
using HireFlow.Application.Features.Cv.Commands.CreateCv;

namespace HireFlow.Application.Common.Validators.Cv;

public class CreateCvCommandValidator : AbstractValidator<CreateCvCommand>
{
	public CreateCvCommandValidator()
	{
		RuleFor(x => x.Request.Title)
			.NotEmpty().WithMessage("CV title is required.")
			.MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

		RuleFor(x => x.Request.Summary)
			.MaximumLength(2000).WithMessage("Summary must not exceed 2000 characters.")
			.When(x => x.Request.Summary is not null);

		RuleFor(x => x.Request.Skills)
			.MaximumLength(1000).WithMessage("Skills must not exceed 1000 characters.")
			.When(x => x.Request.Skills is not null);

		RuleFor(x => x.Request.YearsOfExperience)
			.GreaterThanOrEqualTo(0).WithMessage("Years cannot be negative.")
			.LessThan(50).WithMessage("Years of experience seems unrealistic.")
			.When(x => x.Request.YearsOfExperience is not null);

		RuleFor(x => x.Request.PortfolioUrl)
			.Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
			.WithMessage("Portfolio URL must be a valid URL.")
			.When(x => x.Request.PortfolioUrl is not null);
	}
}
