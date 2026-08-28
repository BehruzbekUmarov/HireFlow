namespace HireFlow.Domain.Primitives;

public abstract class Entity : IEquatable<Entity>
{
	public long Id { get; protected set; }

	public static bool operator ==(Entity? left, Entity? right) =>
		left is null ? right is null : left.Equals(right);

	public static bool operator !=(Entity? left, Entity? right) => !(left == right);

	public bool Equals(Entity? other)
	{
		if (other is null) return false;
		if (ReferenceEquals(this, other)) return true;
		if (GetType() != other.GetType()) return false;

		// Entities not yet persisted (Id == default) have no identity to compare by,
		// so two unsaved instances are never considered equal to one another.
		if (IsTransient() || other.IsTransient()) return false;

		return Id == other.Id;
	}

	public override bool Equals(object? obj) => Equals(obj as Entity);

	public override int GetHashCode() =>
		IsTransient() ? base.GetHashCode() : HashCode.Combine(GetType(), Id);

	private bool IsTransient() => Id == default;
}
