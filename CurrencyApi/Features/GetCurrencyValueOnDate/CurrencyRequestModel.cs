using System;
using CurrencyApi.Errors;

namespace CurrencyApi.Features.GetCurrencyValueOnDate
{
    public enum CurrencyRequestType
    {
        DayBeforeYesterday,
        Yesterday,
        Today,
        Tomorrow
    };

    internal static class CurrencyRequestTypeExtensions
    {
        public static DateTime GetOnDate(this CurrencyRequestType ctr)
        {
            return ctr == CurrencyRequestType.Today ? DateTime.Today :
                ctr == CurrencyRequestType.Yesterday ? DateTime.Today.AddDays(-1) :
                ctr == CurrencyRequestType.DayBeforeYesterday ? DateTime.Today.AddDays(-2) :
                throw new InvalidCurrencyOperationException(ErrorType.InvalidInputData, "Currency value on date specified doesn't exist yet.");
        }
    }

    public class CurrencyRequestModel
    {
        public string CurrencyCode { get; set; }
        public CurrencyRequestType CurrencyRequestType { get; set; }
    }
}
