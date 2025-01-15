namespace Library.Backend.Utils;

public class CustomException : Exception
{
    public CustomException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
}