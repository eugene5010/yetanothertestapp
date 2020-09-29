using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CurrencyApi.Errors;
using MediatR;
using Microsoft.Extensions.Options;

namespace CurrencyApi.Features.GetCurrencyValueOnDate
{
    internal class CurrencyOnDateHandler : IRequestHandler<CurrencyOnDateRequest, CurrencyResponseModel>
    {
        private readonly IOptions<AppSettings> _options;
        //HACK: Simplified cache model without expiration policy and primitive locking
        private static readonly Dictionary<CurrencyRequestType, Dictionary<string, decimal>> CurrencyCache = new Dictionary<CurrencyRequestType, Dictionary<string, decimal>>();
        private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

        public CurrencyOnDateHandler(IOptions<AppSettings> options)
        {
            _options = options;
        }

        public async Task<CurrencyResponseModel> Handle(CurrencyOnDateRequest request, CancellationToken cancellationToken)
        {
            if (request.CurrencyRequestModel == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(request.CurrencyRequestModel.CurrencyCode))
            {
                throw new ArgumentOutOfRangeException(nameof(request.CurrencyRequestModel.CurrencyCode));
            }

            var onDate = request.CurrencyRequestModel.CurrencyRequestType.GetOnDate();
            if (!CurrencyCache.ContainsKey(request.CurrencyRequestModel.CurrencyRequestType))
            {
                await SemaphoreSlim.WaitAsync(cancellationToken);
                try
                {
                    var loaded =  await CurrencyOnDateHelper.LoadCurrencies(_options.Value.BankApiUri, onDate);
                    CurrencyCache.Add(request.CurrencyRequestModel.CurrencyRequestType, loaded);
                }
                finally
                {
                    SemaphoreSlim.Release();
                }
            }

            if (CurrencyCache[request.CurrencyRequestModel.CurrencyRequestType].ContainsKey(request.CurrencyRequestModel.CurrencyCode))
            {
                return new CurrencyResponseModel()
                {
                    CurrencyCode = request.CurrencyRequestModel.CurrencyCode,
                    CurrencyValue = CurrencyCache[request.CurrencyRequestModel.CurrencyRequestType][request.CurrencyRequestModel.CurrencyCode],
                    OnDate = onDate
                };
            }
            
            throw new InvalidCurrencyOperationException(ErrorType.InformationNotFound);
        }
    }
}
