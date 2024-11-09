using Microsoft.AspNetCore.Components;
using static Mx.Blazor.DApp.Client.Application.Constants.MultiversxNetwork;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using static Mx.NET.SDK.TransactionsManager.EGLDTransactionRequest;
using static Mx.NET.SDK.TransactionsManager.TokenTransactionRequest;
using static Mx.NET.SDK.TransactionsManager.ESDTTransactionRequest;
using Microsoft.JSInterop;
using AccToken = Mx.NET.SDK.Domain.Data.Accounts.AccountToken;
using Mx.NET.SDK.Domain.Data.Accounts;

namespace Mx.Blazor.DApp.Client.Pages
{
    public partial class AccountInfo
    {
        [CascadingParameter] private bool WalletConnected { get; set; }

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

        private async Task DisplayAccountInformation()
        {
            var address = WalletProvider.WalletAddress;
            await AccountContainer.Initialize(address);

            StateHasChanged();
        }

        private async void SignMessage()
        {
            if (string.IsNullOrWhiteSpace(SignableMessageText)) return;

            var signMessage = await WalletProvider.SignMessage(SignableMessageText);

            if (signMessage == null)
                await JsRuntime.InvokeVoidAsync("alert", "Signing message operation cancelled.");
            else if (signMessage == true)
                await JsRuntime.InvokeVoidAsync("alert", "Message was successfully signed.");
        }

        private async void SignTransaction()
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
                    Message == "" ? null : Message
                );

                var hashes = await WalletProvider.SignAndSendTransactions("EGLD transaction", transaction);
                if (hashes == null)
                    return;

                Receiver = EGLDAmount = Message = string.Empty;
                StateHasChanged();
                await JsRuntime.InvokeVoidAsync("alert", "EGLD Transaction was sent to the network");
            }
            catch (Exception ex)
            {
                await JsRuntime.InvokeVoidAsync("alert", ex.Message);
            }
        }

        private async void SignTokenTransaction()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ReceiverToken) ||
                    string.IsNullOrWhiteSpace(TokenIdentifier) ||
                    string.IsNullOrWhiteSpace(TokenAmount))
                    return;

                await AccountContainer.SyncAccount();

                var token = AccToken.From(
                    await Provider.GetAccountToken(AccountContainer.Account?.Address.Bech32, TokenIdentifier)
                );

                var transaction = TokenTransfer(
                    NetworkConfig,
                    AccountContainer.Account,
                    Address.FromBech32(ReceiverToken),
                    ESDTIdentifierValue.From(TokenIdentifier),
                    ESDTAmount.ESDT(TokenAmount, token.GetESDT())
                );

                var hashes = await WalletProvider.SignAndSendTransactions("Token transaction", transaction);
                if (hashes == null)
                    return;

                Receiver = EGLDAmount = Message = string.Empty;
                StateHasChanged();
                await JsRuntime.InvokeVoidAsync("alert", "Token Transaction was sent to the network");
            }
            catch (Exception ex)
            {
                await JsRuntime.InvokeVoidAsync("alert", ex.Message);
            }
        }

        private async void SignNftTransaction()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ReceiverNft) || string.IsNullOrWhiteSpace(NftsIdentifier)) return;

                await AccountContainer.SyncAccount();

                var nftsIdentifier = NftsIdentifier.Split([","], StringSplitOptions.RemoveEmptyEntries);
                var args = new List<Tuple<ESDTIdentifierValue, ulong, ESDTAmount>>();
                foreach (var nftIdentifier in nftsIdentifier)
                {
                    var nft = AccountNFT.From(
                        await Provider.GetAccountNFT(AccountContainer.Account?.Address.Bech32, nftIdentifier)
                    );
                    args.Add(
                        new Tuple<ESDTIdentifierValue, ulong, ESDTAmount>(
                            nft.Collection,
                            nft.Nonce,
                            ESDTAmount.ESDT(1, nft.GetESDT())
                        )
                    );
                }

                var transaction = MultiNFTTransfer(
                    NetworkConfig,
                    AccountContainer.Account,
                    Address.FromBech32(ReceiverNft),
                    args.ToArray()
                );

                var hashes = await WalletProvider.SignAndSendTransactions("NFTs transaction", transaction);
                if (hashes == null)
                    return;

                Receiver = EGLDAmount = Message = string.Empty;
                StateHasChanged();
                await JsRuntime.InvokeVoidAsync("alert", "NFTs Transaction was sent to the network");
            }
            catch (Exception ex)
            {
                await JsRuntime.InvokeVoidAsync("alert", ex.Message);
            }
        }

        private async void NewTxExecuted()
        {
            await AccountContainer.SyncAll();

            StateHasChanged();
        }

        private void NewTransactionsExecuted(string[] hashes)
        {
            //do something
        }
    }
}
