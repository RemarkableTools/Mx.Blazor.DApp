using System.Net.Http.Headers;
using System.Text;
using Blazored.SessionStorage;
using Mx.Blazor.DApp.Client.Application.Exceptions;
using Mx.Blazor.DApp.Shared.Models;
using Newtonsoft.Json;
using static Mx.Blazor.DApp.Client.Application.Constants.BrowserStorage;

namespace Mx.Blazor.DApp.Client.Services
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> GetAsync(string? requestUri);
        Task<T> GetAsync<T>(string? requestUri);

        Task<HttpResponseMessage> PostAsync(string? requestUri, object? value = null);
        Task<T> PostAsync<T>(string? requestUri, object? value = null);

        Task<HttpResponseMessage> PutAsync(string? requestUri, object? value = null);
        Task<T> PutAsync<T>(string? requestUri, object? value = null);
    }

    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ISyncSessionStorageService _sessionStorage;

        public HttpService(
            HttpClient httpClient,
            ISyncSessionStorageService sessionStorage)
        {
            _httpClient = httpClient;
            _sessionStorage = sessionStorage;
        }

        public async Task<HttpResponseMessage> GetAsync(string? requestUri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            return await _httpClient.GetAsync(request.RequestUri);
        }

        public async Task<T> GetAsync<T>(string? requestUri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            return await SendRequest<T>(request);
        }

        public async Task<HttpResponseMessage> PostAsync(string? requestUri, object? value = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            if (value != null)
                request.Content = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");

            return await SendRequest(request);
        }

        public async Task<T> PostAsync<T>(string? requestUri, object? value = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            if (value != null)
                request.Content = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");

            return await SendRequest<T>(request);
        }

        public async Task<HttpResponseMessage> PutAsync(string? requestUri, object? value = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, requestUri);
            if (value != null)
                request.Content = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");

            return await SendRequest(request);
        }

        public async Task<T> PutAsync<T>(string? requestUri, object? value = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, requestUri);
            if (value != null)
                request.Content = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");

            return await SendRequest<T>(request);
        }

        private void ApplyHeaders(ref HttpRequestMessage request)
        {
            try
            {
                var accessToken = _sessionStorage.GetItemAsString(ACCESS_TOKEN);
                var isApiUrl = !request.RequestUri.IsAbsoluteUri;
                if (accessToken != null && isApiUrl)
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            catch { }
        }

        private async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request)
        {
            ApplyHeaders(ref request);

            using var response = await _httpClient.SendAsync(request);

            //throw exception on error response
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var httpResponse = JsonConvert.DeserializeObject<HttpResponse>(content) ?? new HttpResponse();
                throw new HttpException(httpResponse);
            }

            return response;
        }

        private async Task<T> SendRequest<T>(HttpRequestMessage request)
        {
            if (typeof(T).Equals(typeof(HttpResponseMessage)))
                throw new Exception("Cannot get HttpResponseMessage with this method. Use Task<HttpResponseMessage> SendRequest(HttpRequestMessage request);");

            ApplyHeaders(ref request);

            using var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            //throw exception on error response
            if (!response.IsSuccessStatusCode)
            {
                var httpResponse = JsonConvert.DeserializeObject<HttpResponse>(content) ?? new HttpResponse();
                throw new HttpException(httpResponse);
            }


            T? deserializedContent;
            if (typeof(T).Equals(typeof(string)))
                deserializedContent = GetValue<T>(content);
            else
                deserializedContent = JsonConvert.DeserializeObject<T>(content);

            //throw exception on null content
            if (deserializedContent is null)
                throw new HttpException(new HttpResponse()
                {
                    StatusCode = response.StatusCode,
                    Message = "Response content is null",
                    Error = "Null content"
                });

            return deserializedContent;
        }

        public static T GetValue<T>(string value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
