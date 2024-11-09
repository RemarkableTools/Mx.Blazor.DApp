using Mx.Blazor.DApp.Shared.Models;
using System.Net;

namespace Mx.Blazor.DApp.Client.Application.Exceptions
{
    public class HttpException(HttpResponse httpResponse) : Exception(httpResponse.Message)
    {
        public HttpStatusCode StatusCode { get; set; } = httpResponse.StatusCode;
        public string Error { get; set; } = httpResponse.Error;
    }
}
