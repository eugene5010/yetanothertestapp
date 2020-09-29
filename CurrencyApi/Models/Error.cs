using CurrencyApi.Errors;
using Newtonsoft.Json;

namespace CurrencyApi.Models
{
    public class Error
    {
        [JsonProperty("type")]
        public ErrorType Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
