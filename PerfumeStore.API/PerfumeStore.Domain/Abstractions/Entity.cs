namespace PerfumeStore.Domain.Abstractions
{
    public abstract class Entity
    {
        public int Id { get; protected set; }

        protected Entity()
        {
        }
    }
}