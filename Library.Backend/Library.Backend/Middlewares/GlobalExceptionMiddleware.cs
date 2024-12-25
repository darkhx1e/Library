using Library.Backend.Data;

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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var statusCode = ex switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        var errorResponse = new ApiErrorResponse
        {
            StatusCode = statusCode,
            Message = ex.Message,
            Details = statusCode == StatusCodes.Status500InternalServerError ? "Internal Server Error" : null
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsJsonAsync(errorResponse);
    }
}