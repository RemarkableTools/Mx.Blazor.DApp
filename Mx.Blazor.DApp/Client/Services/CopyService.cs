using Microsoft.JSInterop;

namespace Mx.Blazor.DApp.Client.Services
{
    public interface ICopyService
    {
        Task CopyToClipboard(string text);
    }

    public class CopyService(IJSRuntime jsRuntime) : ICopyService
    {
        public async Task CopyToClipboard(string text)
        {
            await jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
        }
    }
}
