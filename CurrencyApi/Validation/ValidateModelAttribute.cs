using System.Linq;
using CurrencyApi.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CurrencyApi.Validation
{
    internal class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (actionContext.ModelState.IsValid == false)
                actionContext.Result = new BadRequestObjectResult(
                    new
                    {
                        Errors = actionContext.ModelState.Keys
                            .SelectMany(key => actionContext.ModelState[key].Errors.Select(x => new
                            {
                                Code = (int)ErrorType.InvalidInputData,
                                Message = $"{key}: {x.ErrorMessage}"
                            })).ToList()
                    });
        }
    }
}
