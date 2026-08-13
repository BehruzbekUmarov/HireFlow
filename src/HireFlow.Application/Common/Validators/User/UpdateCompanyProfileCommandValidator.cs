using FluentValidation;
using HireFlow.Application.Features.Common.User.Commands.UpdateCompanyProfile;

namespace HireFlow.Application.Common.Validators.User;

public class UpdateCompanyProfileCommandValidator
	: AbstractValidator<UpdateCompanyProfileCommand>
{
	public UpdateCompanyProfileCommandValidator()
	{
		RuleFor(x => x.Request.Name)
			.NotEmpty().WithMessage("Company name is required.")
			.MinimumLength(2).WithMessage("Company name must be at least 2 characters.")
			.MaximumLength(200).WithMessage("Company name must not exceed 200 characters.");

		RuleFor(x => x.Request.Description)
			.MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.")
			.When(x => x.Request.Description is not null);

		RuleFor(x => x.Request.Website)
			.MaximumLength(500).WithMessage("Website must not exceed 500 characters.")
			.Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
			.WithMessage("Website must be a valid URL.")
			.When(x => x.Request.Website is not null);

		RuleFor(x => x.Request.Location)
			.MaximumLength(200).WithMessage("Location must not exceed 200 characters.")
			.When(x => x.Request.Location is not null);
	}
}