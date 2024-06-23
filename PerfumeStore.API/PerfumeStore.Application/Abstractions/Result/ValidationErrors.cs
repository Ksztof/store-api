using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Application.Abstractions.Result
{
    public static class ValidationErrors
    {
        public static Error FormValidationError(string propertyName, string errorMessage) =>
            Error.Validation($"{propertyName}.FormValidationError", $"{errorMessage}");
    }
}
