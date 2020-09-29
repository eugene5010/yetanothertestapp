using System;
using System.ComponentModel.DataAnnotations;
using CurrencyApi.Models;
using Microsoft.Extensions.Options;

namespace CurrencyApi.Validation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class CoordinateValidationAttribute : ValidationAttribute
    {
        private readonly string _xCoordinatePropertyName;
        private readonly string _yCoordinatePropertyName;

        public CoordinateValidationAttribute(string xCoordinatePropertyName, string yCoordinatePropertyName)
        {
            _xCoordinatePropertyName = xCoordinatePropertyName;
            _yCoordinatePropertyName = yCoordinatePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is CurrencyGeoModel))
            {
                throw new ArgumentOutOfRangeException(nameof(CoordinateValidationAttribute));
            }

            var xCoordinatePropertyName = validationContext.ObjectType.GetProperty(_xCoordinatePropertyName);
            var yCoordinatePropertyName = validationContext.ObjectType.GetProperty(_yCoordinatePropertyName);

            if (xCoordinatePropertyName == null || yCoordinatePropertyName == null)
            {
                throw new ArgumentOutOfRangeException(nameof(CoordinateValidationAttribute));
            }

            var xCoordinateValue = (int)xCoordinatePropertyName.GetValue(value);
            var yCoordinateValue = (int)yCoordinatePropertyName.GetValue(value);

            var options = (IOptions<AppSettings>)validationContext
                .GetService(typeof(IOptions<AppSettings>));
            
            var radius = options?.Value.Radius ?? 0;

            if (xCoordinateValue * xCoordinateValue + yCoordinateValue * yCoordinateValue > radius * radius)
            {
                return new ValidationResult("Coordinate X and Y placed over the circle radius");
            }

            return ValidationResult.Success;
        }
    }
}
