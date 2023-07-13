namespace PerfumeStore.Core.CustomExceptions
{
    public class CartNotFoundException : Exception
    {
        public CartNotFoundException(string message)
            : base(message)
        {
        }

        public CartNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
