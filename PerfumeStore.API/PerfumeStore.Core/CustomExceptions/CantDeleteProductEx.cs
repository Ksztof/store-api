using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.CustomExceptions
{
    public class CantDeleteProductEx : Exception
    {
        public CantDeleteProductEx(string message)
            : base(message) { }
        public CantDeleteProductEx(string message, Exception cantDeleteProductEx)
           : base(message, cantDeleteProductEx) { }
    }
}
