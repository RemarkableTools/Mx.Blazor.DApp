using System.Net;

namespace Mx.Blazor.DApp.Shared.Models
{
    public class HttpResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = "";
        public string Error { get; set; } = "";

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
