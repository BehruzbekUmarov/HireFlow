using FluentValidation;
using HireFlow.Application.Features.Common.Auth.Commands.ResetPassword;

namespace HireFlow.Application.Common.Validators.Auth;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
	public ResetPasswordCommandValidator()
	{
		RuleFor(x => x.Request.Code)
			.NotEmpty().WithMessage("Reset code is required.")
			.Length(6).WithMessage("Reset code must be 6 digits.")
			.Matches("^[0-9]+$").WithMessage("Reset code must contain only digits.");

		RuleFor(x => x.Request.NewPassword)
			.NotEmpty().WithMessage("New password is required.")
			.MinimumLength(8).WithMessage("Password must be at least 8 characters.")
			.Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
			.Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
			.Matches("[0-9]").WithMessage("Password must contain at least one number.");

		RuleFor(x => x.Request.ConfirmPassword)
			.NotEmpty().WithMessage("Confirm password is required.")
			.Equal(x => x.Request.NewPassword).WithMessage("Passwords do not match.");
	}
}