using Microsoft.AspNetCore.Components;
using Mx.Blazor.DApp.Client.Application.Constants;
using Mx.Blazor.DApp.Client.Services.Containers;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain.Data.Network;
using Mx.NET.SDK.TransactionsManager;

namespace Mx.Blazor.DApp.Client.Pages
{
    public partial class AccountInfo
    {
        [CascadingParameter]
        private bool WalletConnected { get; set; }

        private string SignableMessageText { get; set; } = "";

        private string Receiver { get; set; } = "";
        private string EGLDAmount { get; set; } = "";
        private string Message { get; set; } = "";
        private List<string> HashesExecuted { get; set; } = new();

        protected override void OnInitialized()
        {
            WalletProvider.OnWalletConnected += OnWalletConnected;
            TransactionsContainer.TxExecuted += SyncAccount;
            TransactionsContainer.HashesExecuted += NewTransactionsExecuted;
        }

        protected override async Task OnInitializedAsync()
        {
            if (!WalletConnected) return;

            await DisplayAccountInformation();
        }

        private async void OnWalletConnected()
        {
            await DisplayAccountInformation();
        }

        public async Task DisplayAccountInformation()
        {
            var address = WalletProvider.GetAddress();
            await AccountContainer.Initialize(address);

            StateHasChanged();
        }

        public async void SyncAccount()
        {
            await AccountContainer.Sync();

            StateHasChanged();
        }

        public async void SignMessage()
        {
            if (string.IsNullOrWhiteSpace(SignableMessageText)) return;

            var signMessage = await WalletProvider.SignMessage(new SignableMessage() { Message = SignableMessageText });
            Console.WriteLine(signMessage);
        }

        public async void SignTransaction()
        {
            if (string.IsNullOrWhiteSpace(Receiver) || string.IsNullOrWhiteSpace(EGLDAmount)) return;

            var provider = MultiversxNetwork.Provider;
            var networkConfig = await NetworkConfig.GetFromNetwork(provider);
            await AccountContainer.SyncAccount();

            var transaction = EGLDTransactionRequest.EGLDTransfer(
            networkConfig,
            AccountContainer.Account,
            Address.FromBech32(Receiver),
            ESDTAmount.EGLD(EGLDAmount),
            Message == "" ? null : Message);

            await WalletProvider.SignTransaction(transaction, "One transaction");
        }

        public void NewTransactionsExecuted(string[] hashes)
        {
            //do something
        }
    }
}
