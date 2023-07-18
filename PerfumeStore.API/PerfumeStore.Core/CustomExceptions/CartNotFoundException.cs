namespace PerfumeStore.Core.CustomExceptions
{
    public class CartNotFoundException : Exception
    {
        public CartNotFoundException(string message)
            : base(message) { }
        public CartNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
