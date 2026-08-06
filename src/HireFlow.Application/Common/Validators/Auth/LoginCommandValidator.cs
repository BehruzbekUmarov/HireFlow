using FluentValidation;
using HireFlow.Application.Features.Common.Commands.Login;

namespace HireFlow.Application.Common.Validators.Auth;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
	public LoginCommandValidator()
	{
		RuleFor(x => x.Request.Email)
			.NotEmpty().WithMessage("Email is required.")
			.EmailAddress().WithMessage("Email format is invalid.");

		RuleFor(x => x.Request.Password)
			.NotEmpty().WithMessage("Password is required.");
	}
}