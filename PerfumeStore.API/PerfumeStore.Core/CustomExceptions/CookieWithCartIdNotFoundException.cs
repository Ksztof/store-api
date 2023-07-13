namespace PerfumeStore.Core.CustomExceptions
{
    public class CookieWithCartIdNotFoundException : Exception
    {
        public CookieWithCartIdNotFoundException()
        {
        }

        public CookieWithCartIdNotFoundException(string message)
            : base(message)
        {
        }

        public CookieWithCartIdNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
