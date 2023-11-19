namespace PerfumeStore.Application.CustomExceptions
{
    public class MissingClaimInTokenEx : Exception
    {
        public string MissingClaimType { get; }

        public MissingClaimInTokenEx(string missingClaimType)
            : base($"The claim of type '{missingClaimType}' is missing.")
        {
            MissingClaimType = missingClaimType;
        }

        public MissingClaimInTokenEx(string missingClaimType, Exception ex)
           : base($"The claim of type '{missingClaimType}' is missing.", ex)
        {
            MissingClaimType = missingClaimType;
        }
    }
}
