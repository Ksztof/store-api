namespace PerfumeStore.Application.CustomExceptions
{
    public class RequestForUserEx : Exception
    {
        public RequestForUserEx(string message) : base(message)
        {
        }

        public RequestForUserEx(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
