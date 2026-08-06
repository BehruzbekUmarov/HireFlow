namespace HireFlow.Domain.Exceptions;

public class CompanyNotApprovedException : DomainException
{
	public CompanyNotApprovedException()
		: base("Your company account is pending admin approval and cannot post jobs yet.") { }
}
