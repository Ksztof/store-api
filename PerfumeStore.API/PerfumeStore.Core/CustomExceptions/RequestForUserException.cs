using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.CustomExceptions
{
  public class RequestForUserException: Exception
  {
    public RequestForUserException(string message) : base(message)
    {
    }

    public RequestForUserException(string message, Exception ex) : base(message, ex)
    {
    }
  }
}
