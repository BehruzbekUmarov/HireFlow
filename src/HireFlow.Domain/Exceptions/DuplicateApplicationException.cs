namespace HireFlow.Domain.Exceptions;

public class DuplicateApplicationException : DomainException
{
	public DuplicateApplicationException()
		: base("You have already applied to this job.") { }
}
