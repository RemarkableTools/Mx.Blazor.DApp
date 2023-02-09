using Mx.Blazor.DApp.Client.Application.Exceptions;
using Mx.Blazor.DApp.Client.Application.ExtensionMethods;
using Mx.Blazor.DApp.Client.Services.WalletProviders.Interfaces;
using Microsoft.JSInterop;
using Mx.NET.SDK.Domain;

namespace Mx.Blazor.DApp.Client.Services.WalletProviders
{
    public class ExtensionWalletProvider : IWalletProvider
    {
        private readonly IJSRuntime JsRuntime;
        public ExtensionWalletProvider(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }

        public async Task Init(params string[] args)
        {
            var initialized = await JsRuntime.InvokeAsync<bool>("ExtensionWallet.Obj.init", args);
            if (!initialized)
                throw new InitException();
        }

        public async Task<string> Login(string authToken)
        {
            return await JsRuntime.InvokeAsync<string>("ExtensionWallet.Obj.login", authToken);
        }

        public async Task<string> GetAddress()
        {
            return await JsRuntime.InvokeAsync<string>("ExtensionWallet.Obj.getAddress");
        }

        public async Task<bool> IsConnected()
        {
            return await JsRuntime.InvokeAsync<bool>("ExtensionWallet.Obj.isConnected");
        }

        public async Task Logout()
        {
            await JsRuntime.InvokeVoidAsync("ExtensionWallet.Obj.logout");
        }

        public async Task TransactionIsCanceled()
        {
            await JsRuntime.InvokeVoidAsync("ExtensionWallet.Obj.transactionCanceled");
        }

        public async Task<string> SignTransaction(TransactionRequest transactionRequest)
        {
            return await JsRuntime.InvokeAsync<string>("ExtensionWallet.Obj.signTransaction", transactionRequest.GetTransactionRequestDecoded());
        }

        public async Task<string> SignTransactions(TransactionRequest[] transactionsRequest)
        {
            return await JsRuntime.InvokeAsync<string>("ExtensionWallet.Obj.signTransactions", (object)transactionsRequest.GetTransactionsRequestDecoded());
        }
    }
}
