using System;
using CurrencyApi.Features.GetCurrencyValueOnDate;
using CurrencyApi.Validation;

namespace CurrencyApi.Models
{
    [CoordinateValidation(nameof(X), nameof(Y))]
    public class CurrencyGeoModel
    {
        public int X { get; set; }
        public int Y { get; set; }

        public CurrencyRequestType GetCurrencyRequestType()
        {
            return this.X > 0 && this.Y > 0 ? CurrencyRequestType.Today :
                this.X < 0 && this.Y > 0 ? CurrencyRequestType.Yesterday :
                this.X < 0 && this.Y < 0 ? CurrencyRequestType.DayBeforeYesterday :
                this.X > 0 && this.Y < 0 ? CurrencyRequestType.Tomorrow : 
                throw new ArgumentOutOfRangeException("Currency geomodel is in unsupported state in the context of operation");
        }
    }
}
