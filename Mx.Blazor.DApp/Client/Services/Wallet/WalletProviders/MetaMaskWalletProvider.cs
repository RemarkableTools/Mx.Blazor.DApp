using Mx.Blazor.DApp.Client.Application.Exceptions;
using Mx.Blazor.DApp.Client.Application.ExtensionMethods;
using Microsoft.JSInterop;
using Mx.NET.SDK.Domain;
using Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders.Interfaces;

namespace Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders
{
    public class MetaMaskWalletProvider : IWalletProvider
    {
        private readonly IJSRuntime JsRuntime;
        public MetaMaskWalletProvider(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }

        public async Task Init(params string[] args)
        {
            var initialized = await JsRuntime.InvokeAsync<bool>("MetaMaskWallet.Obj.init", args);
            if (!initialized)
                throw new InitException();
        }

        public async Task<string> Login(string authToken)
        {
            return await JsRuntime.InvokeAsync<string>("MetaMaskWallet.Obj.login", authToken);
        }

        public async Task<string> GetAddress()
        {
            return await JsRuntime.InvokeAsync<string>("MetaMaskWallet.Obj.getAddress");
        }

        public async Task<bool> IsConnected()
        {
            return await JsRuntime.InvokeAsync<bool>("MetaMaskWallet.Obj.isConnected");
        }

        public async Task Logout()
        {
            await JsRuntime.InvokeVoidAsync("MetaMaskWallet.Obj.logout");
        }

        public async Task TransactionIsCanceled()
        {
            await JsRuntime.InvokeVoidAsync("MetaMaskWallet.Obj.transactionCanceled");
        }

        public async Task<string> SignMessage(string message)
        {
            return await JsRuntime.InvokeAsync<string>("MetaMaskWallet.Obj.signMessage", message);
        }

        public async Task<string> SignTransaction(TransactionRequest transactionRequest)
        {
            return await JsRuntime.InvokeAsync<string>("MetaMaskWallet.Obj.signTransaction", transactionRequest.GetTransactionRequestDecoded());
        }

        public async Task<string> SignTransactions(TransactionRequest[] transactionsRequest)
        {
            return await JsRuntime.InvokeAsync<string>("MetaMaskWallet.Obj.signTransactions", (object)transactionsRequest.GetTransactionsRequestDecoded());
        }

        public async Task CancelAction()
        {
            await JsRuntime.InvokeVoidAsync("MetaMaskWallet.Obj.cancelAction");
        }
    }
}
