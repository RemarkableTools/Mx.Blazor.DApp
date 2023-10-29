using Mx.Blazor.DApp.Client.Application.ExtensionMethods;
using Microsoft.JSInterop;
using Mx.Blazor.DApp.Client.Application.Exceptions;
using Mx.NET.SDK.Domain;
using Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders.Interfaces;

namespace Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders
{
    public class WebWalletProvider : IWalletProvider
    {
        private readonly IJSRuntime JsRuntime;
        public WebWalletProvider(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }

        public async Task Init(params string[] args)
        {
            var initialized = await JsRuntime.InvokeAsync<bool>("WebWallet.Obj.init", args);
            if (!initialized)
                throw new InitException();
        }

        public async Task<string> Login(string authToken)
        {
            await JsRuntime.InvokeVoidAsync("WebWallet.Obj.login", authToken);
            return "";
        }

        public async Task<string> GetAddress()
        {
            return await JsRuntime.InvokeAsync<string>("WebWallet.Obj.getAddress");
        }

        public async Task<bool> IsConnected()
        {
            return await JsRuntime.InvokeAsync<bool>("WebWallet.Obj.isConnected");
        }

        public async Task Logout()
        {
            await JsRuntime.InvokeVoidAsync("WebWallet.Obj.logout");
        }

        public async Task TransactionIsCanceled()
        {
            await JsRuntime.InvokeVoidAsync("WebWallet.Obj.transactionCanceled");
        }

        public async Task<string> SignMessage(string message)
        {
            return await JsRuntime.InvokeAsync<string>("WebWallet.Obj.signMessage", message);
        }

        public async Task<string> SignTransaction(TransactionRequest transactionRequest)
        {
            return await JsRuntime.InvokeAsync<string>("WebWallet.Obj.signTransaction", transactionRequest.GetTransactionRequestDecoded());
        }

        public async Task<string> SignTransactions(TransactionRequest[] transactionsRequest)
        {
            return await JsRuntime.InvokeAsync<string>("WebWallet.Obj.signTransactions", (object)transactionsRequest.GetTransactionsRequestDecoded());
        }

        public Task CancelAction()
        {
            return Task.CompletedTask;
        }
    }
}
