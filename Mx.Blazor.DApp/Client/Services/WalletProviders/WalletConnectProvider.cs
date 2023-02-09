using Mx.Blazor.DApp.Client.Application.ExtensionMethods;
using Mx.Blazor.DApp.Client.Services.WalletProviders.Interfaces;
using Microsoft.JSInterop;
using Mx.Blazor.DApp.Client.Application.Exceptions;
using Mx.NET.SDK.Domain;

namespace Mx.Blazor.DApp.Client.Services.WalletProviders
{
    public class WalletConnectProvider : IWalletProvider
    {
        private readonly IJSRuntime JsRuntime;
        public WalletConnectProvider(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }

        public async Task Init(params string[] args)
        {
            var initialized = await JsRuntime.InvokeAsync<bool>("WalletConnect.Obj.init");
            if (!initialized)
                throw new InitException();
        }

        public async Task<string> Login(string authToken)
        {
            await JsRuntime.InvokeVoidAsync("WalletConnect.Obj.login", authToken);
            return "";
        }

        public async Task<string> GetAddress()
        {
            return await JsRuntime.InvokeAsync<string>("WalletConnect.Obj.getAddress");
        }

        public async Task<bool> IsConnected()
        {
            return await JsRuntime.InvokeAsync<bool>("WalletConnect.Obj.isConnected");
        }

        public async Task Logout()
        {
            await JsRuntime.InvokeVoidAsync("WalletConnect.Obj.logout");
        }

        public async Task TransactionIsCanceled()
        {
            await JsRuntime.InvokeVoidAsync("WalletConnect.Obj.transactionCanceled");
        }

        public async Task<string> SignTransaction(TransactionRequest transactionRequest)
        {
            return await JsRuntime.InvokeAsync<string>("WalletConnect.Obj.signTransaction", transactionRequest.GetTransactionRequestDecoded());
        }

        public async Task<string> SignTransactions(TransactionRequest[] transactionsRequest)
        {
            return await JsRuntime.InvokeAsync<string>("WalletConnect.Obj.signTransactions", (object)transactionsRequest.GetTransactionsRequestDecoded());
        }
    }
}
