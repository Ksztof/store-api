using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.CustomExceptions
{
    public class ProductCategoryNotFoundEx : Exception
    {
        public ProductCategoryNotFoundEx(string message)
            : base(message) { }
        public ProductCategoryNotFoundEx(string message, Exception innerException)
            : base(message, innerException) { }              
    }
}
