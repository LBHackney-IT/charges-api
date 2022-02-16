using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Domain;
using FluentValidation;

namespace ChargesApi.V1.Infrastructure.Validators
{
    public class AddChargeRequestValidator : AbstractValidator<AddChargeRequest>
    {
        public AddChargeRequestValidator()
        {
            RuleFor(x => x.ChargeSubGroup).NotNull()
                .When(_ => _.ChargeGroup == ChargeGroup.Leaseholders)
                .WithMessage("{PropertyName} should be provided if Charge Group is Leaseholders");
        }
    }
}
