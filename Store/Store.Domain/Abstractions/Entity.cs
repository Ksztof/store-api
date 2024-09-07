namespace Store.Domain.Abstractions;

public abstract class Entity
{
    public int Id { get; protected set; }
    protected Entity() { }
}