using FluentValidation;

namespace HireFlow.Application.Common.Validators.Auth;
public class RegisterFreelancerCommandValidator : AbstractValidator<RegisterFreelancerCommand>
{
	public RegisterFreelancerCommandValidator()
	{
		RuleFor(x => x.Request.Email)
			.NotEmpty().WithMessage("Email is required.")
			.EmailAddress().WithMessage("Email format is invalid.")
			.MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

		RuleFor(x => x.Request.Password)
			.NotEmpty().WithMessage("Password is required.")
			.MinimumLength(8).WithMessage("Password must be at least 8 characters.")
			.MaximumLength(100).WithMessage("Password must not exceed 100 characters.")
			.Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
			.Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
			.Matches("[0-9]").WithMessage("Password must contain at least one number.");

		RuleFor(x => x.Request.FullName)
			.NotEmpty().WithMessage("Full name is required.")
			.MinimumLength(2).WithMessage("Full name must be at least 2 characters.")
			.MaximumLength(200).WithMessage("Full name must not exceed 200 characters.");
	}
}