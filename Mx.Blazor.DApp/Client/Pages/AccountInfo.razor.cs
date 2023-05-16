using Microsoft.AspNetCore.Components;
using static Mx.Blazor.DApp.Client.Application.Constants.MultiversxNetwork;
using Mx.Blazor.DApp.Client.Services.Containers;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using static Mx.NET.SDK.TransactionsManager.EGLDTransactionRequest;
using Mx.Blazor.DApp.Client.Application.Constants;
using Microsoft.JSInterop;

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

        protected override void OnInitialized()
        {
            WalletProvider.OnWalletConnected += OnWalletConnected;
            TransactionsContainer.TxExecuted += NewTxExecuted;
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

        public async void SignMessage()
        {
            if (string.IsNullOrWhiteSpace(SignableMessageText)) return;

            var signMessage = await WalletProvider.SignMessage(new SignableMessage() { Message = SignableMessageText });

            if(signMessage == true)
                await JsRuntime.InvokeVoidAsync("alert", "Message was successfully signed.");
            else
                await JsRuntime.InvokeVoidAsync("alert", "Message signature is invalid.");
        }

        public async void SignTransaction()
        {
            if (string.IsNullOrWhiteSpace(Receiver) || string.IsNullOrWhiteSpace(EGLDAmount)) return;

            await AccountContainer.SyncAccount();

            var transaction = EGLDTransfer(
            NetworkConfig,
            AccountContainer.Account,
            Address.FromBech32(Receiver),
            ESDTAmount.EGLD(EGLDAmount),
            Message == "" ? null : Message);

            //Use the below function before SignAndSend to do a post process after the transaction is sent to the blockchain
            WalletProvider.PreparePostTxSendProcess(
                PostTxSendProcess.ProcessID1,
                "Text to be shown after the TX is signed and sent");
            var hash = await WalletProvider.SignAndSendTransaction(transaction, "One transaction");
            if (hash != null)
            {
                Receiver = EGLDAmount = Message = string.Empty;
                StateHasChanged();
            }
        }

        public async void NewTxExecuted()
        {
            await AccountContainer.SyncAll();

            StateHasChanged();
        }

        public void NewTransactionsExecuted(string[] hashes)
        {
            //do something
        }
    }
}
