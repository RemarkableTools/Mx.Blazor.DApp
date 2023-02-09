using Microsoft.JSInterop;

namespace Mx.Blazor.DApp.Client.Services
{
    public interface ICopyService
    {
        Task CopyToClipboard(string text);
    }

    public class CopyService : ICopyService
    {
        private readonly IJSRuntime JsRuntime;
        public CopyService(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }

        public async Task CopyToClipboard(string text)
        {
            await JsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
        }
    }
}
