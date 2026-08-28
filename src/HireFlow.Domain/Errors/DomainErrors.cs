using HireFlow.Domain.Common;

namespace HireFlow.Domain.Errors;

public static class DomainErrors
{
	public static class Company
	{
		public static readonly Error NotAssociated = new(
			"Company.NotAssociated",
			"You must be associated with a company to post a job.");

		public static readonly Func<long, Error> NotFound = id => new Error(
			"Company.NotFound",
			$"Company with id '{id}' was not found.");

		public static readonly Error NotApproved = new(
			"Company.NotApproved",
			"Your company account is pending admin approval and cannot post jobs yet.");
	}
}
