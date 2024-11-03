using Mx.Blazor.DApp.Client.Application.Exceptions;
using Mx.Blazor.DApp.Client.Application.ExtensionMethods;
using Microsoft.JSInterop;
using Mx.NET.SDK.Domain;
using Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders.Interfaces;
using Mx.NET.SDK.Provider.API;
using Mx.Blazor.DApp.Client.Models;
using Newtonsoft.Json;
using Mx.Blazor.DApp.Shared.Connection;
using Mx.NET.SDK.Core.Domain;

namespace Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders
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

        public async Task<AccountToken> Login(string authToken)
        {
            return await JsRuntime.InvokeAsync<AccountToken>("ExtensionWallet.Obj.login", authToken);
        }

        public async Task<AccountToken> GetAccount()
        {
            return await JsRuntime.InvokeAsync<AccountToken>("ExtensionWallet.Obj.getAccount");
        }

        public async Task Logout()
        {
            await JsRuntime.InvokeVoidAsync("ExtensionWallet.Obj.logout");
        }

        public async Task<string> SignMessage(string message)
        {
            return await JsRuntime.InvokeAsync<string>("ExtensionWallet.Obj.signMessage", message);
        }

        public async Task<string> SignTransaction(TransactionRequest transactionRequest)
        {
            return await JsRuntime.InvokeAsync<string>("ExtensionWallet.Obj.signTransaction", transactionRequest.GetTransactionRequestDecoded());
        }

        public async Task<string> SignTransactions(TransactionRequest[] transactionsRequest)
        {
            return await JsRuntime.InvokeAsync<string>("ExtensionWallet.Obj.signTransactions", (object)transactionsRequest.GetTransactionsRequestDecoded());
        }

        public async Task CancelAction()
        {
            await JsRuntime.InvokeVoidAsync("ExtensionWallet.Obj.cancelAction");
        }
    }
}
