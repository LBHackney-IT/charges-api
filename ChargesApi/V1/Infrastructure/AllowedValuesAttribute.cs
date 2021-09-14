using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ChargesApi.V1.Infrastructure
{
    public class AllowedValuesAttribute : ValidationAttribute
    {
        private readonly Type _type;

        public AllowedValuesAttribute(Type enumType)
        {
            _type = enumType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult($"{validationContext.MemberName} field should not be null");

            var valueType = value.GetType();

            if (!valueType.IsEnum)
            {
                return new ValidationResult($"{validationContext.MemberName} field should be a type of enum.");
            }
            else if (!Enum.IsDefined(_type, value))
            {
                var values = Enum.GetNames(_type);
                return new ValidationResult($"{validationContext.MemberName} field should be a type of {_type.Name} enum. Values: {string.Join(", ", values.Select(a => a))}");
            }

            return ValidationResult.Success;
        }
    }
}
