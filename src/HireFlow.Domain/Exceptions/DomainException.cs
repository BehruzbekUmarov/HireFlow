namespace HireFlow.Domain.Exceptions;

public class DomainException : Exception
{
	public DomainException(string message) : base(message) { }
}

public class CompanyNotApprovedException : DomainException
{
	public CompanyNotApprovedException()
		: base("Your company account is pending admin approval and cannot post jobs yet.") { }
}

public class DuplicateApplicationException : DomainException
{
	public DuplicateApplicationException()
		: base("You have already applied to this job.") { }
}

public class InvalidStatusTransitionException : DomainException
{
	public InvalidStatusTransitionException(string from, string to)
		: base($"Cannot move an application from {from} to {to}.") { }
}
