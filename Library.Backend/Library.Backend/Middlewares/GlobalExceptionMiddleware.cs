using Library.Backend.Utils;

namespace Library.Backend.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (CustomException ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, CustomException ex)
    {
        var errorResponse = new ApiErrorResponse
        {
            StatusCode = ex.StatusCode,
            Message = ex.Message
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = ex.StatusCode;

        return context.Response.WriteAsJsonAsync(errorResponse);
    }
}