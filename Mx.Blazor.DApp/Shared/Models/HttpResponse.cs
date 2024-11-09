using System.Net;

namespace Mx.Blazor.DApp.Shared.Models
{
    public class HttpResponse
    {
        public HttpStatusCode StatusCode { get; init; }
        public string Message { get; init; } = "";
        public string Error { get; init; } = "";

        public HttpResponse() { }

        public HttpResponse(
            HttpStatusCode statusCode,
            string message,
            string error)
        {
            StatusCode = statusCode;
            Message = message;
            Error = error;
        }
    }
}
