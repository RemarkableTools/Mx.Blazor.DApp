using Mx.Blazor.DApp.Client.Application.ExtensionMethods;
using Microsoft.JSInterop;
using Mx.Blazor.DApp.Client.Application.Exceptions;
using Mx.NET.SDK.Domain;
using Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders.Interfaces;

namespace Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders
{
    public class HardwareWalletProvider : IWalletProvider
    {
        private readonly IJSRuntime JsRuntime;
        public HardwareWalletProvider(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }

        public async Task Init(params string[] args)
        {
            var initialized = await JsRuntime.InvokeAsync<bool>("HardwareWallet.Obj.init");
            if (!initialized)
                throw new InitException();
        }

        public async Task<string> Login(string authToken)
        {
            return await JsRuntime.InvokeAsync<string>("HardwareWallet.Obj.login", authToken);
        }

        public async Task<string> GetAddress()
        {
            return await JsRuntime.InvokeAsync<string>("HardwareWallet.Obj.getAddress");
        }

        public async Task<bool> IsConnected()
        {
            return await JsRuntime.InvokeAsync<bool>("HardwareWallet.Obj.isConnected");
        }

        public async Task Logout()
        {
            await JsRuntime.InvokeVoidAsync("HardwareWallet.Obj.logout");
        }

        public async Task TransactionIsCanceled()
        {
            await JsRuntime.InvokeVoidAsync("HardwareWallet.Obj.transactionCanceled");
        }

        public async Task<string> SignMessage(string message)
        {
            return await JsRuntime.InvokeAsync<string>("HardwareWallet.Obj.signMessage", message);
        }

        public async Task<string> SignTransaction(TransactionRequest transactionRequest)
        {
            return await JsRuntime.InvokeAsync<string>("HardwareWallet.Obj.signTransaction", transactionRequest.GetTransactionRequestDecoded());
        }

        public async Task<string> SignTransactions(TransactionRequest[] transactionsRequest)
        {
            return await JsRuntime.InvokeAsync<string>("HardwareWallet.Obj.signTransactions", (object)transactionsRequest.GetTransactionsRequestDecoded());
        }

        public Task CancelAction()
        {
            return Task.CompletedTask;
        }
    }
}
