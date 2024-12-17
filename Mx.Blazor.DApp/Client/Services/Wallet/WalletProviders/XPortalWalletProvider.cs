using Mx.Blazor.DApp.Client.Application.ExtensionMethods;
using Microsoft.JSInterop;
using Mx.Blazor.DApp.Client.Application.Exceptions;
using Mx.NET.SDK.Domain;
using static Mx.Blazor.DApp.Client.Application.Constants.MultiversxNetwork;
using Mx.NET.SDK.Configuration;
using Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders.Interfaces;
using Mx.Blazor.DApp.Shared.Connection;
using Mx.NET.SDK.Provider.Dtos.Common.Transactions;

namespace Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders
{
    public class XPortalWalletProvider(IJSRuntime jsRuntime) : IWalletProvider
    {
        private static string GetNetwork()
        {
            return Provider.NetworkConfiguration.Network switch
            {
                Network.LocalNet => "local",
                Network.MainNet => "1",
                Network.DevNet => "D",
                Network.TestNet => "T",
                _ => throw new Exception("Network doesn't exist!")
            };
        }

        public async Task Init(params object?[]? args)
        {
            var initialized = await jsRuntime.InvokeAsync<bool>("XPortalWallet.Obj.init", GetNetwork());
            if (!initialized)
                throw new InitException();
        }

        public async Task<AccountToken> Login(string authToken)
        {
            return await jsRuntime.InvokeAsync<AccountToken>("XPortalWallet.Obj.login", authToken);
        }

        public async Task Logout()
        {
            await jsRuntime.InvokeVoidAsync("XPortalWallet.Obj.logout");
        }

        public async Task<string> SignMessage(string message)
        {
            return await jsRuntime.InvokeAsync<string>("XPortalWallet.Obj.signMessage", message);
        }

        public async Task<string> SignTransactions(TransactionRequest[] transactionsRequest)
        {
            return await jsRuntime.InvokeAsync<string>(
                "XPortalWallet.Obj.signTransactions",
                (object)transactionsRequest.GetTransactionsRequestDecoded()
            );
        }

        public async Task<string> SignTransactions(TransactionRequestDto[] transactionsRequest)
        {
            return await jsRuntime.InvokeAsync<string>(
                "XPortalWallet.Obj.signTransactions",
                (object)transactionsRequest
            );
        }

        public Task CancelAction()
        {
            return Task.CompletedTask;
        }
    }
}
