using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.CustomExceptions
{
  public class MissingClaimInTokenException : Exception
  {
    public string MissingClaimType { get; }

    public MissingClaimInTokenException(string missingClaimType)
        : base($"The claim of type '{missingClaimType}' is missing.")
    {
      MissingClaimType = missingClaimType;
    }

    public MissingClaimInTokenException(string missingClaimType, Exception ex)
       : base($"The claim of type '{missingClaimType}' is missing.", ex)
    {
      MissingClaimType = missingClaimType;
    }
  }
}
