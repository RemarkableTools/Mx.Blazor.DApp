using Mx.Blazor.DApp.Client.Application.Exceptions;
using Mx.Blazor.DApp.Client.Application.ExtensionMethods;
using Microsoft.JSInterop;
using Mx.NET.SDK.Domain;
using Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders.Interfaces;
using Mx.Blazor.DApp.Shared.Connection;
using Mx.NET.SDK.Core.Domain;

namespace Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders
{
    public class CrossWindowWalletProvider : IWalletProvider
    {
        private readonly IJSRuntime JsRuntime;
        public CrossWindowWalletProvider(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }

        public async Task Init(params string[] args)
        {
            var initialized = await JsRuntime.InvokeAsync<bool>("CrossWindowWallet.Obj.init", args);
            if (!initialized)
                throw new InitException();
        }

        public async Task<AccountToken> Login(string authToken)
        {
            return await JsRuntime.InvokeAsync<AccountToken>("CrossWindowWallet.Obj.login", authToken);
        }

        public async Task<string> GetAddress()
        {
            return await JsRuntime.InvokeAsync<string>("CrossWindowWallet.Obj.getAddress");
        }

        public async Task<bool> IsConnected()
        {
            return await JsRuntime.InvokeAsync<bool>("CrossWindowWallet.Obj.isConnected");
        }

        public async Task Logout()
        {
            await JsRuntime.InvokeVoidAsync("CrossWindowWallet.Obj.logout");
        }

        public async Task TransactionIsCanceled()
        {
            await JsRuntime.InvokeVoidAsync("CrossWindowWallet.Obj.transactionCanceled");
        }

        public async Task<string> SignMessage(string message)
        {
            return await JsRuntime.InvokeAsync<string>("CrossWindowWallet.Obj.signMessage", message);
        }

        public async Task<string> SignTransaction(TransactionRequest transactionRequest)
        {
            return await JsRuntime.InvokeAsync<string>("CrossWindowWallet.Obj.signTransaction", transactionRequest.GetTransactionRequestDecoded());
        }

        public async Task<string> SignTransactions(TransactionRequest[] transactionsRequest)
        {
            return await JsRuntime.InvokeAsync<string>("CrossWindowWallet.Obj.signTransactions", (object)transactionsRequest.GetTransactionsRequestDecoded());
        }

        public async Task CancelAction()
        {
            await JsRuntime.InvokeVoidAsync("CrossWindowWallet.Obj.cancelAction");
        }
    }
}
