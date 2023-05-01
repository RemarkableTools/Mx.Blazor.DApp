using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Mx.Blazor.DApp.Client.Application.Constants;
using static Mx.Blazor.DApp.Client.Application.Constants.BrowserSessionStorage;
using Mx.Blazor.DApp.Client.Services.Wallet;
using Microsoft.JSInterop;

namespace Mx.Blazor.DApp.Client.Services
{
    public class PostTxSendService
    {
        private readonly IHttpService Http;
        private readonly ISyncSessionStorageService _sessionStorage;
        private readonly IJSRuntime JsRuntime;
        public PostTxSendService(
            IHttpService http,
            ISyncSessionStorageService sessionStorage,
            IJSRuntime jsRuntime)
        {
            Http = http;
            _sessionStorage = sessionStorage;
            JsRuntime = jsRuntime;
        }

        public async Task Run()
        {
            switch (_sessionStorage.GetItem<PostTxSendProcess>(POST_PROCESS))
            {
                case PostTxSendProcess.ProcessID1:
                    await DoProcessID1();
                    break;
            }
            _sessionStorage.RemoveItem(POST_PROCESS);
            _sessionStorage.RemoveItem(POST_PROCESS_OBJECT);
        }

        private async Task DoProcessID1()
        {
            var message = _sessionStorage.GetItem<string>(POST_PROCESS_OBJECT);
            await JsRuntime.InvokeVoidAsync("alert", message);
        }

        private async Task ExampleDatabaseUpdate()
        {
            try
            {
                //Call API
            }
            catch (UnauthorizedAccessException)
            {
                await JsRuntime.InvokeVoidAsync("alert", "Your access token expired. You have been disconnected from wallet.");
                WalletManagerService.Logout();
            }
        }
    }
}
