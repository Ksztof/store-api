namespace PerfumeStore.Application.HttpContext
{
    public interface IHttpContextService
    {
        public bool IsUserAuthenticated();
        public string GetUserId();
        public string GetActualProtocol();
    }
}