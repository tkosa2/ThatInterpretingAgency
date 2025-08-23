using System.Net;

namespace BlazorFrontend.Services;

public class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string? ResponseContent { get; }

    public ApiException(HttpStatusCode statusCode, string message, string? responseContent = null) 
        : base(message)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
    }
}
