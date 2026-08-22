using FluentValidation;
using HireFlow.Application.Features.Chat.Queries.GetConversation;

namespace HireFlow.Application.Common.Validators.Chat;

public class GetConversationQueryValidator : AbstractValidator<GetConversationQuery>
{
	public GetConversationQueryValidator()
	{
		RuleFor(x => x.ApplicationId)
			.GreaterThan(0).WithMessage("Application ID must be valid.");
	}
}