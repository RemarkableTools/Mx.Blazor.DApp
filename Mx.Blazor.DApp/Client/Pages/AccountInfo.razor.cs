using Microsoft.AspNetCore.Components;
using static Mx.Blazor.DApp.Client.Application.Constants.MultiversxNetwork;
using Mx.Blazor.DApp.Client.Services.Containers;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using static Mx.NET.SDK.TransactionsManager.EGLDTransactionRequest;
using static Mx.NET.SDK.TransactionsManager.TokenTransactionRequest;
using static Mx.NET.SDK.TransactionsManager.ESDTTransactionRequest;
using Mx.Blazor.DApp.Client.Application.Constants;
using Microsoft.JSInterop;
using AccToken = Mx.NET.SDK.Domain.Data.Accounts.AccountToken;
using Mx.NET.SDK.Domain.Data.Accounts;

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

        private string ReceiverToken { get; set; } = "";
        private string TokenIdentifier { get; set; } = "";
        private string TokenAmount { get; set; } = "";

        private string ReceiverNft { get; set; } = "";
        private string NftsIdentifier { get; set; } = "";

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
            var address = WalletProvider.WalletAddress;
            await AccountContainer.Initialize(address);

            StateHasChanged();
        }

        public async void SignMessage()
        {
            if (string.IsNullOrWhiteSpace(SignableMessageText)) return;

            var signMessage = await WalletProvider.SignMessage(SignableMessageText);

            if (signMessage == null)
                await JsRuntime.InvokeVoidAsync("alert", "Signing message operation cancelled.");
            else  if (signMessage == true)
                await JsRuntime.InvokeVoidAsync("alert", "Message was successfully signed.");
        }

        public async void SignTransaction()
        {
            try
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
                //After tx is sent the process will start: function RunPostTxSendProcess() in WalletProviderContainer
                WalletProvider.PreparePostTxSendProcess(
                    PostTxSendProcess.ProcessID1,
                    "EGLD Transaction was sent to the network");
                var hash = await WalletProvider.SignAndSendTransaction(transaction, "EGLD transaction");
                if (hash != null)
                {
                    Receiver = EGLDAmount = Message = string.Empty;
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                await JsRuntime.InvokeVoidAsync("alert", ex.Message);
            }
        }

        public async void SignTokenTransaction()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ReceiverToken) || string.IsNullOrWhiteSpace(TokenIdentifier) || string.IsNullOrWhiteSpace(TokenAmount))
                    return;

                await AccountContainer.SyncAccount();

                var token = AccToken.From(await Provider.GetAccountToken(AccountContainer.Account.Address.Bech32, TokenIdentifier));

                var transaction = TokenTransfer(
                    NetworkConfig,
                    AccountContainer.Account,
                    Address.FromBech32(ReceiverToken),
                    ESDTIdentifierValue.From(TokenIdentifier),
                    ESDTAmount.ESDT(TokenAmount, token.GetESDT()));

                //Use the below function before SignAndSend to do a post process after the transaction is sent to the blockchain
                //After tx is sent the process will start: function RunPostTxSendProcess() in WalletProviderContainer
                WalletProvider.PreparePostTxSendProcess(
                    PostTxSendProcess.ProcessID1,
                    "Token Transaction was sent to the network");
                var hash = await WalletProvider.SignAndSendTransaction(transaction, "Token transaction");
                if (hash != null)
                {
                    Receiver = EGLDAmount = Message = string.Empty;
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                await JsRuntime.InvokeVoidAsync("alert", ex.Message);
            }
        }

        public async void SignNftTransaction()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ReceiverNft) || string.IsNullOrWhiteSpace(NftsIdentifier)) return;

                await AccountContainer.SyncAccount();

                var nftsIdentifer = NftsIdentifier.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var args = new List<Tuple<ESDTIdentifierValue, ulong, ESDTAmount>>();
                foreach (var nftIdentifier in nftsIdentifer)
                {
                    var nft = AccountNFT.From(await Provider.GetAccountNFT(AccountContainer.Account.Address.Bech32, nftIdentifier));
                    args.Add(new Tuple<ESDTIdentifierValue, ulong, ESDTAmount>(nft.Collection, nft.Nonce, ESDTAmount.ESDT(1, nft.GetESDT())));
                }

                var transaction = MultiNFTTransfer(
                    NetworkConfig,
                    AccountContainer.Account,
                    Address.FromBech32(ReceiverNft),
                    args.ToArray());

                //Use the below function before SignAndSend to do a post process after the transaction is sent to the blockchain
                //After tx is sent the process will start: function RunPostTxSendProcess() in WalletProviderContainer
                WalletProvider.PreparePostTxSendProcess(
                    PostTxSendProcess.ProcessID1,
                    "NFTs Transaction was sent to the network");
                var hash = await WalletProvider.SignAndSendTransaction(transaction, "NFTs transaction");
                if (hash != null)
                {
                    Receiver = EGLDAmount = Message = string.Empty;
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                await JsRuntime.InvokeVoidAsync("alert", ex.Message);
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
