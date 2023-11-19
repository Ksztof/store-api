using FluentValidation;

namespace PerfumeStore.Application.Validators
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
