using Microsoft.JSInterop;
using Mx.Blazor.DApp.Client.Application.Exceptions;
using Mx.Blazor.DApp.Client.Application.ExtensionMethods;
using Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders.Interfaces;
using Mx.NET.SDK.Domain;

namespace Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders
{
    public class WebViewProvider : IWalletProvider
    {
        private readonly IJSRuntime JsRuntime;
        public WebViewProvider(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }

        public async Task Init(params string[] args)
        {
            var initialized = await JsRuntime.InvokeAsync<bool>("WebView.Obj.init", args);
            if (!initialized)
                throw new InitException();
        }

        public async Task<string> Login(string authToken)
        {
            return await JsRuntime.InvokeAsync<string>("WebView.Obj.login");
        }

        public async Task<string> GetAddress()
        {
            return await JsRuntime.InvokeAsync<string>("WebView.Obj.getAddress");
        }

        public async Task<bool> IsConnected()
        {
            return await JsRuntime.InvokeAsync<bool>("WebView.Obj.isConnected");
        }

        public async Task Logout()
        {
            await JsRuntime.InvokeVoidAsync("WebView.Obj.logout");
        }

        public async Task TransactionIsCanceled()
        {
            await JsRuntime.InvokeVoidAsync("WebView.Obj.transactionCanceled");
        }

        public async Task<string> SignMessage(string message)
        {
            return await JsRuntime.InvokeAsync<string>("WebView.Obj.signMessage", message);
        }

        public async Task<string> SignTransaction(TransactionRequest transactionRequest)
        {
            return await JsRuntime.InvokeAsync<string>("WebView.Obj.signTransaction", transactionRequest.GetTransactionRequestDecoded());
        }

        public async Task<string> SignTransactions(TransactionRequest[] transactionsRequest)
        {
            return await JsRuntime.InvokeAsync<string>("WebView.Obj.signTransactions", (object)transactionsRequest.GetTransactionsRequestDecoded());
        }

        public Task CancelAction()
        {
            return Task.CompletedTask;
        }
    }
}
