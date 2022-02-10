using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Domain;
using FluentValidation;

namespace ChargesApi.V1.Infrastructure.Validators
{
    public class UpdateChargeRequestValidator : AbstractValidator<UpdateChargeRequest>
    {
        public UpdateChargeRequestValidator()
        {
            RuleFor(x => x.ChargeSubGroup).NotNull()
                .When(_ => _.ChargeGroup == ChargeGroup.Tenants)
                .WithMessage("{PropertyName} should be provided if Charge Group is Tenants");
        }
    }
}
