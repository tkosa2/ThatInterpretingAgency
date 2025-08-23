using System;

namespace ThatInterpretingAgency.Core.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }

    protected Entity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    protected Entity(Guid id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    protected void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (obj is not Entity other) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity? left, Entity? right)
    {
        return EqualOperator(left, right);
    }

    public static bool operator !=(Entity? left, Entity? right)
    {
        return NotEqualOperator(left, right);
    }

    protected static bool EqualOperator(Entity? left, Entity? right)
    {
        if (left is null ^ right is null)
        {
            return false;
        }
        return left?.Equals(right) != false;
    }

    protected static bool NotEqualOperator(Entity? left, Entity? right)
    {
        return !EqualOperator(left, right);
    }
}
