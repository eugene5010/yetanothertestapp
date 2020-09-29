using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using CurrencyApi.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CurrencyApi.Middleware
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IOptions<JsonOptions> jsonOptions;
        private readonly ILogger<CustomExceptionMiddleware> logger;

        public CustomExceptionMiddleware(
            RequestDelegate next,
            IOptions<JsonOptions> jsonOptions,
            ILoggerFactory loggerFactory)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));

            this.jsonOptions = jsonOptions;

            logger = loggerFactory?.CreateLogger<CustomExceptionMiddleware>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                logger.LogError(exception, "Error while processing request {requestDescription} from {user}:{newline}",
                    FormatRequest(context.Request),
                    context.User.Identity.Name,
                    Environment.NewLine);

                var title = (exception as InvalidCurrencyOperationException)?.ErrorType.ToString() ?? ErrorType.UnknownError.ToString();
                await context.Response.WriteAsync(JsonSerializer.Serialize(
                    new ProblemDetails
                    {
                        Title = title,
                        Detail = exception.ToString(),
                        Status = context.Response.StatusCode,
                    },
                    jsonOptions.Value.JsonSerializerOptions));
            }
        }

        private string FormatRequest(HttpRequest request)
        {
            return $"{request.Scheme}://{request.Host}{request.Path}";
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class HttpStatusCodeExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionMiddleware>();
        }
    }
}
