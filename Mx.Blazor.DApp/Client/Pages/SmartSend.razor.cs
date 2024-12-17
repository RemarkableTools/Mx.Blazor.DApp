using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Mx.Blazor.DApp.Shared;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Provider.Dtos.Common.Transactions;
using Mx.NET.SDK.TransactionsManager;
using static Mx.Blazor.DApp.Client.Application.Constants.MultiversxNetwork;

namespace Mx.Blazor.DApp.Client.Pages;

public partial class SmartSend
{
    [CascadingParameter]
    private bool WalletConnected { get; set; }

    private bool _loading;

    private string InputMessage { get; set; } = string.Empty;
    private string SmartSendAddress { get; set; } = "erd1qqqqqqqqqqqqqpgqf63uhw0jkqt3fy9u8n938hhz237ms0de4drq0rxpd2";

    protected override void OnInitialized()
    {
        WalletProvider.OnWalletConnected += OnWalletConnected;
    }

    void IDisposable.Dispose()
    {
        WalletProvider.OnWalletConnected -= OnWalletConnected;
        GC.SuppressFinalize(this);
    }

    protected override async Task OnInitializedAsync()
    {
        if (WalletConnected)
            await SyncPage();
    }

    private async void OnWalletConnected()
    {
        try
        {
            await SyncPage();
        }
        catch { }
    }

    private async Task SyncPage()
    {
        await AccountContainer.Initialize(WalletProvider.WalletAddress);
        StateHasChanged();
    }

    private async Task SubmitRequest()
    {
        // send request to the bot
        // get the response
        _loading = true;
        StateHasChanged();

        await AccountContainer.SyncAccount();

        var executorAddress = await Http.PostAsync<string>(
            $"api/transactions/generate-wallet/{AccountContainer.Account!.Shard}"
        );

        // var esdtToken = ESDT.TOKEN("MVX-d30f4d", "MVX-d30f4d", 18);
        // const int nrOfTxs = 300;
        // var args = new List<IBinaryType>
        // {
        //     Address.FromBech32(executorAddress)
        // };
        // for (var i = 0; i < nrOfTxs; i++)
        // {
        //     args.Add(Address.FromBech32("erd1ysrfrcysz54460rhmvqm43rn7jmugkh2zl5eahmywn9yap55hfkq0sjqzy"));
        //     args.Add(NumericValue.BigUintValue(ESDTAmount.ESDT(1, esdtToken).Value));
        // }
        //
        // var transactions = TokenTransactionRequest.MultiTokensTransferToSmartContract(
        //     NetworkConfig,
        //     AccountContainer.Account!,
        //     Address.FromBech32(SmartSendAddress),
        //     new GasLimit(1500000 * nrOfTxs),
        //     [
        //         new Tuple<ESDTIdentifierValue, ESDTAmount>(
        //             ESDTIdentifierValue.From("WEGLD-a28c59"),
        //             ESDTAmount.ESDT("0.02", ESDT.TOKEN("WEGLD-a28c59", "WEGLD-a28c59", 18))
        //         ),
        //         new Tuple<ESDTIdentifierValue, ESDTAmount>(
        //             ESDTIdentifierValue.From("TET-d81442"),
        //             ESDTAmount.ESDT(nrOfTxs, esdtToken)
        //         )
        //     ],
        //     "smartSave",
        //     args.ToArray()
        // );
        //
        // await Task.Delay(1000);

        var agentRequest = new AgentRequest
        {
            InputMessage = InputMessage,
            ChainId = NetworkConfig!.ChainId,
            Sender = AccountContainer.Account!.Address.Bech32,
            ContractAddress = SmartSendAddress,
            ServiceAddress = executorAddress,
        };

        try
        {
            var response = await Http.PostAsync<string>(
                "https://agent.com/airdrop",
                agentRequest
            );
            var transaction = JsonWrapper.Deserialize<TransactionRequestDto>(response);

            _loading = false;
            StateHasChanged();

            var hashes = await WalletProvider.SignAndSendTransactions(
                "Smart Send transactions",
                transaction
            );
            if (hashes != null && hashes.Length != 0)
            {

                var transactionsRequest = new TransactionsRequest
                {
                    InitiatorAddress = AccountContainer.Account!.Address.Bech32,
                    ExecutorAddress = executorAddress,
                    ContractAddress = SmartSendAddress,
                    NrOfTransactions = 1,
                    ScheduledTime = DateTime.UtcNow + TimeSpan.FromSeconds(10),
                };
                await Http.PostAsync("api/transactions/schedule-transactions", transactionsRequest);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await JsRuntime.InvokeVoidAsync("alert", "An error occured while submitting the request");
        }
        finally
        {
            InputMessage = string.Empty;
            _loading = false;
            StateHasChanged();
        }
    }
}
