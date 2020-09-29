using System;
using CurrencyApi.Errors;
using MediatR;

namespace CurrencyApi.Features.GetCurrencyValueOnDate
{
    internal class CurrencyOnDateRequest : IRequest<CurrencyResponseModel>
    {
        public CurrencyRequestModel CurrencyRequestModel { get; set; }
    }
}
