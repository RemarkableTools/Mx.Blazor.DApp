using Mx.Blazor.DApp.Client.Application.ExtensionMethods;
using Mx.Blazor.DApp.Client.Services.WalletProviders.Interfaces;
using Microsoft.JSInterop;
using Mx.Blazor.DApp.Client.Application.Exceptions;
using Mx.NET.SDK.Domain;
using Mx.Blazor.DApp.Client.Application.Constants;
using Mx.NET.SDK.Configuration;

namespace Mx.Blazor.DApp.Client.Services.WalletProviders
{
    public class XPortalProvider : IWalletProvider
    {
        private readonly IJSRuntime JsRuntime;
        public XPortalProvider(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }

        private static string GetNetwork()
        {
            return MultiversxNetwork.Provider.NetworkConfiguration.Network switch
            {
                Network.LocalNet => "local",
                Network.MainNet => "1",
                Network.DevNet => "D",
                Network.TestNet => "T",
                _ => throw new Exception("Network doesn't exist!")
            };
        }

        public async Task Init(params string[] args)
        {
            var initialized = await JsRuntime.InvokeAsync<bool>("XPortalWallet.Obj.init", GetNetwork());
            if (!initialized)
                throw new InitException();
        }

        public async Task<string> Login(string authToken)
        {
            await JsRuntime.InvokeVoidAsync("XPortalWallet.Obj.login", authToken);
            return "";
        }

        public async Task<string> GetAddress()
        {
            return await JsRuntime.InvokeAsync<string>("XPortalWallet.Obj.getAddress");
        }

        public async Task<bool> IsConnected()
        {
            return await JsRuntime.InvokeAsync<bool>("XPortalWallet.Obj.isConnected");
        }

        public async Task Logout()
        {
            await JsRuntime.InvokeVoidAsync("XPortalWallet.Obj.logout");
        }

        public async Task TransactionIsCanceled()
        {
            await JsRuntime.InvokeVoidAsync("XPortalWallet.Obj.transactionCanceled");
        }

        public async Task<string> SignTransaction(TransactionRequest transactionRequest)
        {
            return await JsRuntime.InvokeAsync<string>("XPortalWallet.Obj.signTransaction", transactionRequest.GetTransactionRequestDecoded());
        }

        public async Task<string> SignTransactions(TransactionRequest[] transactionsRequest)
        {
            return await JsRuntime.InvokeAsync<string>("XPortalWallet.Obj.signTransactions", (object)transactionsRequest.GetTransactionsRequestDecoded());
        }
    }
}
