namespace Library.Backend.Utils;

public class ApiErrorResponse
{
    public int StatusCode { get; set; }
    public required string Message { get; set; }
}