namespace PerfumeStore.Domain.Repositories.Generics
{
    public interface IEntity<TId>
    {
        TId Id { get; set; }
    }
}