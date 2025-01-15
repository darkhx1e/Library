using Library.Backend.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Library.Backend.Filters;

public class ValidationExceptionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(ms => ms.Value.Errors.Count > 0)
                .SelectMany(ms => ms.Value.Errors.Select(e => e.ErrorMessage))
                .ToArray();

            var message = string.Join(" ", errors);

            var response = new ApiErrorResponse
            {
                StatusCode = 400,
                Message = message
            };

            context.Result = new JsonResult(response)
            {
                StatusCode = 400
            };
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}