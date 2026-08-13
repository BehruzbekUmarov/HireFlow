using FluentValidation;
using HireFlow.Application.Features.Common.User.Commands.UpdateFreelancerProfile;

namespace HireFlow.Application.Common.Validators.User;

public class UpdateFreelancerProfileCommandValidator
	: AbstractValidator<UpdateFreelancerProfileCommand>
{
	public UpdateFreelancerProfileCommandValidator()
	{
		RuleFor(x => x.Request.Bio)
			.MaximumLength(1000).WithMessage("Bio must not exceed 1000 characters.")
			.When(x => x.Request.Bio is not null);

		RuleFor(x => x.Request.Skills)
			.MaximumLength(500).WithMessage("Skills must not exceed 500 characters.")
			.When(x => x.Request.Skills is not null);

		RuleFor(x => x.Request.PhoneNumber)
			.MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.")
			.Matches(@"^\+?[0-9\s\-\(\)]+$").WithMessage("Phone number format is invalid.")
			.When(x => x.Request.PhoneNumber is not null);

		RuleFor(x => x.Request.PortfolioUrl)
			.MaximumLength(500).WithMessage("Portfolio URL must not exceed 500 characters.")
			.Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
			.WithMessage("Portfolio URL must be a valid URL.")
			.When(x => x.Request.PortfolioUrl is not null);

		RuleFor(x => x.Request.YearsOfExperience)
			.GreaterThanOrEqualTo(0).WithMessage("Years of experience cannot be negative.")
			.LessThan(50).WithMessage("Years of experience seems unrealistic.")
			.When(x => x.Request.YearsOfExperience is not null);
	}
}