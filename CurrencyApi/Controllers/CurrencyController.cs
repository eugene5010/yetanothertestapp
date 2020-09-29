using System.Threading.Tasks;
using CurrencyApi.Features.GetCurrencyValueOnDate;
using CurrencyApi.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CurrencyApi.Controllers
{
    [ApiController]
    [Route("currencies/selected-currency")]
    [Produces("application/json")]
    public class CurrencyController
    {
        private readonly IMediator _mediator;
        private readonly IOptions<AppSettings> _settings;

        public CurrencyController(IMediator mediator, IOptions<AppSettings> settings)
        {
            _mediator = mediator;
            _settings = settings;
        }

        [HttpPost]
        [Route("values/current")]
        public async Task<CurrencyResponseModel> GetCurrencyValue(CurrencyGeoModel request) =>
            await _mediator.Send(new CurrencyOnDateRequest()
            {
                CurrencyRequestModel = new CurrencyRequestModel()
                {
                    CurrencyRequestType = request.GetCurrencyRequestType(),
                    CurrencyCode = _settings.Value.CurrencyCode
                }
            });
    }
}
