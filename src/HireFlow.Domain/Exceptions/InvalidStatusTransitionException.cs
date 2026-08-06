namespace HireFlow.Domain.Exceptions;

public class InvalidStatusTransitionException : DomainException
{
	public InvalidStatusTransitionException(string from, string to)
		: base($"Cannot move an application from {from} to {to}.") { }
}
