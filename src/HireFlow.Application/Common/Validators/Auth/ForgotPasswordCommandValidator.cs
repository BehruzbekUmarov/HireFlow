using FluentValidation;
using HireFlow.Application.Features.Common.Commands.ForgetPassword;

namespace HireFlow.Application.Common.Validators.Auth;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
	public ForgotPasswordCommandValidator()
	{
		RuleFor(x => x.Request.Email)
			.NotEmpty().WithMessage("Email is required.")
			.EmailAddress().WithMessage("Email format is invalid.");
	}
}
