using System;

namespace CurrencyApi.Features.GetCurrencyValueOnDate
{
    public class CurrencyResponseModel
    {
        public string CurrencyCode { get; set; }
        public decimal CurrencyValue { get; set; }
        public DateTime OnDate { get; set; }
    }
}
