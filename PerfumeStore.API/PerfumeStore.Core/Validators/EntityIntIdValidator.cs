using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Validators
{
    public class EntityIntIdValidator : AbstractValidator<int>
    {
        public EntityIntIdValidator()
        {

            RuleFor(x => x)
                .GreaterThan(0)
                    .WithMessage("Value must be greater than 0.");
        }
    }
}
