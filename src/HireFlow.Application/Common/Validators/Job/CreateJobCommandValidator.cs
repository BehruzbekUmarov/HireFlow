using FluentValidation;
using HireFlow.Application.Features.Job.Commands.CreateJob;

namespace HireFlow.Application.Common.Validators.Job;

public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{
	public CreateJobCommandValidator()
	{
		RuleFor(x => x.Request.Title)
			.NotEmpty().WithMessage("Job title is required.")
			.MinimumLength(5).WithMessage("Title must be at least 5 characters.")
			.MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

		RuleFor(x => x.Request.Description)
			.NotEmpty().WithMessage("Job description is required.")
			.MinimumLength(20).WithMessage("Description must be at least 20 characters.")
			.MaximumLength(5000).WithMessage("Description must not exceed 5000 characters.");

		RuleFor(x => x.Request.Category)
			.NotEmpty().WithMessage("Category is required.")
			.MaximumLength(100).WithMessage("Category must not exceed 100 characters.");

		RuleFor(x => x.Request.Location)
			.NotEmpty().WithMessage("Location is required.")
			.MaximumLength(200).WithMessage("Location must not exceed 200 characters.");

		RuleFor(x => x.Request.Salary)
			.GreaterThan(0).WithMessage("Salary must be greater than 0.")
			.LessThan(1_000_000).WithMessage("Salary seems unrealistically high.");
	}
}
