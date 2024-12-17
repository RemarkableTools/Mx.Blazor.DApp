using Mx.Blazor.DApp.Client.Application.Exceptions;
using Mx.Blazor.DApp.Client.Application.ExtensionMethods;
using Microsoft.JSInterop;
using Mx.NET.SDK.Domain;
using Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders.Interfaces;
using Mx.Blazor.DApp.Shared.Connection;
using Mx.NET.SDK.Provider.Dtos.Common.Transactions;

namespace Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders
{
    public class CrossWindowWalletProvider(IJSRuntime jsRuntime) : IWalletProvider
    {
        public async Task Init(params object?[]? args)
        {
            var initialized = await jsRuntime.InvokeAsync<bool>("CrossWindowWallet.Obj.init", args);
            if (!initialized)
                throw new InitException();
        }

        public async Task<AccountToken> Login(string authToken)
        {
            return await jsRuntime.InvokeAsync<AccountToken>("CrossWindowWallet.Obj.login", authToken);
        }

        public async Task Logout()
        {
            await jsRuntime.InvokeVoidAsync("CrossWindowWallet.Obj.logout");
        }

        public async Task<string> SignMessage(string message)
        {
            return await jsRuntime.InvokeAsync<string>("CrossWindowWallet.Obj.signMessage", message);
        }

        public async Task<string> SignTransactions(TransactionRequest[] transactionsRequest)
        {
            return await jsRuntime.InvokeAsync<string>(
                "CrossWindowWallet.Obj.signTransactions",
                (object)transactionsRequest.GetTransactionsRequestDecoded()
            );
        }

        public async Task<string> SignTransactions(TransactionRequestDto[] transactionsRequest)
        {
            return await jsRuntime.InvokeAsync<string>(
                "CrossWindowWallet.Obj.signTransactions",
                (object)transactionsRequest
            );
        }

        public async Task CancelAction()
        {
            await jsRuntime.InvokeVoidAsync("CrossWindowWallet.Obj.cancelAction");
        }
    }
}
