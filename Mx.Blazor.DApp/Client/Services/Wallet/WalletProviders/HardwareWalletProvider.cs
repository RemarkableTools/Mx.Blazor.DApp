using Mx.Blazor.DApp.Client.Application.ExtensionMethods;
using Microsoft.JSInterop;
using Mx.Blazor.DApp.Client.Application.Exceptions;
using Mx.NET.SDK.Domain;
using Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders.Interfaces;
using Mx.Blazor.DApp.Shared.Connection;
using Mx.NET.SDK.Core.Domain.Helper;

namespace Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders
{
    public class HardwareWalletProvider(IJSRuntime jsRuntime) : IWalletProvider
    {
        public async Task Init(params object?[]? args)
        {
            var initialized = await jsRuntime.InvokeAsync<bool>("HardwareWallet.Obj.init");
            if (!initialized)
                throw new InitException();
        }

        public async Task<AccountToken> Login(string authToken)
        {
            var result = await jsRuntime.InvokeAsync<object>("HardwareWallet.Obj.login", authToken);
            return JsonWrapper.Deserialize<AccountToken>(result.ToString());
        }

        public async Task Logout()
        {
            await jsRuntime.InvokeVoidAsync("HardwareWallet.Obj.logout");
        }

        public async Task<string> SignMessage(string message)
        {
            return await jsRuntime.InvokeAsync<string>("HardwareWallet.Obj.signMessage", message);
        }

        public async Task<string> SignTransactions(TransactionRequest[] transactionsRequest)
        {
            return await jsRuntime.InvokeAsync<string>(
                "HardwareWallet.Obj.signTransactions",
                (object)transactionsRequest.GetTransactionsRequestDecoded()
            );
        }

        public Task CancelAction()
        {
            return Task.CompletedTask;
        }
    }
}
