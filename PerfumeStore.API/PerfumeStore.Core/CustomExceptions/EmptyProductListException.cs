using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.CustomExceptions
{
    public class EmptyProductListException :Exception
    {
        public EmptyProductListException(string message)
            : base(message) { }
        public EmptyProductListException(string message, Exception emptyProductListException)
            : base(message, emptyProductListException) { }
    }
}
