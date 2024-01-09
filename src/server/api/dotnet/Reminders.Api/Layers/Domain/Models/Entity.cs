namespace Reminders.Domain.Models;

public abstract class Entity<TId>
{
    public TId Id { get; protected set; }
    public bool IsDeleted { get; protected set; }

    protected Entity(
        TId id,
        bool isDeleted)
    {
        if (Equals(id, default(TId)))
            throw new ArgumentException("The ID cannot be the type's default value", "id");

        Id = id;
        IsDeleted = isDeleted;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Entity() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public override bool Equals(object? obj)
    {
        var entity = obj as Entity<TId>;

        if (entity is not null) Equals(entity);

        return base.Equals(obj);
    }

    public static bool operator ==(Entity<TId> a, Entity<TId> b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity<TId> a, Entity<TId> b) => !(a == b);

    public override int GetHashCode()
    {
        if (Id is not null)
            return GetType().GetHashCode() * 907 + Id.GetHashCode();

        return default;
    }

    public override string ToString() => GetType().Name + " [Id=" + Id + "]";

    public void Delete() => IsDeleted = true;
}