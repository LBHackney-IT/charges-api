using ChargeApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ChargeApi.V1.Infrastructure
{
    public class AllowedMaintenanceStatusAttribute:ValidationAttribute
    {
        private readonly List<ChargeMaintenanceStatus> _allowedEnumItems;

        public AllowedMaintenanceStatusAttribute(params ChargeMaintenanceStatus[] allowedEnumItems)
        {
            _allowedEnumItems = allowedEnumItems.ToList();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult($"{validationContext.MemberName} is required.");
            }

            var valueType = value.GetType();

            if (!valueType.IsEnum || !Enum.IsDefined(typeof(ChargeMaintenanceStatus), value))
            {
                return new ValidationResult($"{validationContext.MemberName} should be a type of ChargesMaintenanceStatus enum.");
            }

            var isValid = _allowedEnumItems.Contains((ChargeMaintenanceStatus) value);

            if (isValid)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult($"{validationContext.MemberName} should be in a range: [{string.Join(", ", _allowedEnumItems.Select(a => $"{(int) a}({a})"))}].");
            }
        }
    }
}
