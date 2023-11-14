namespace PerfumeStore.Core.CustomExceptions
{
    public class RequestForUserException : Exception
    {
        public RequestForUserException(string message) : base(message)
        {
        }

        public RequestForUserException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
