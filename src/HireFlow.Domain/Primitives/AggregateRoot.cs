namespace HireFlow.Domain.Primitives;

public abstract class AggregateRoot : Entity
{
	private readonly List<IDomainEvent> _domainEvents = [];

	public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();

	public void ClearDomainEvents() => _domainEvents.Clear();

	protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
