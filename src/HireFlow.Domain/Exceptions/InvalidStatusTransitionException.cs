namespace HireFlow.Domain.Exceptions;

public class InvalidStatusTransitionException : Exception
{
	public InvalidStatusTransitionException(string from, string to)
		: base($"Cannot move an application from {from} to {to}.") { }
}
