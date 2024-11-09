using Microsoft.AspNetCore.Components;
using static Mx.Blazor.DApp.Client.Application.Constants.MultiversxNetwork;
using static Mx.Blazor.DApp.Client.Application.Constants.DAppConstants;
using Mx.Blazor.DApp.Client.Models;
using Mx.NET.SDK.Provider.Dtos.API.Transactions;

namespace Mx.Blazor.DApp.Client.Shared.Components.Transactions
{
    public partial class AutomatedTx
    {
        [Parameter] public TransactionModel TransactionModel { get; set; } = default!;

        [Parameter] public EventCallback<TransactionModel> Update { get; set; }

        [Parameter] public EventCallback<TransactionModel> Dismiss { get; set; }

        private readonly CancellationTokenSource _syncToken = new();

        protected override async Task OnInitializedAsync()
        {
            var cancellationToken = _syncToken.Token;
            await Task.Factory.StartNew(
                async () =>
                {
                    if (TransactionModel.Transactions.All(
                            t => t.Status != "pending"
                        )) // this is called only on page refresh
                        goto ALLEXECUTED;

                    await Task.Delay(TxCheckTime, cancellationToken); // delay before checking the transactions statuses

                    GETTXS:
                    var apiTransactions = await GetTransactions(); // get the transactions from API
                    foreach (var transactionData in TransactionModel.Transactions)
                    {
                        var data = transactionData;
                        var apiTransaction = apiTransactions.SingleOrDefault(t => t.TxHash == data.Hash);
                        transactionData.Status = apiTransaction != null ? apiTransaction.Status : "invalid";
                    }

                    StateHasChanged(); // update UI for transactions in the model
                    await Update.InvokeAsync(TransactionModel); // update stored transaction model

                    if (TransactionModel.Transactions.Any(
                            t => t.Status == "pending"
                        )) // there are still transactions pending
                    {
                        await Task.Delay(TxCheckTime, cancellationToken);
                        goto GETTXS;
                    }

                    TransactionsContainer.TransactionsExecuted(
                        TransactionModel.Transactions.Select(t => t.Hash).ToArray()
                    );
                    StateHasChanged();

                    ALLEXECUTED:
                    if (TxDismissTime > 0)
                    {
                        if (TransactionModel.Transactions.Find(tx => tx.Status != "success") == null)
                        {
                            await Task.Delay(TxDismissTime, cancellationToken);
                            await Dismiss.InvokeAsync(TransactionModel);
                        }
                    }
                },
                cancellationToken
            );
        }

        private async Task<TransactionDto[]> GetTransactions()
        {
            var @params = new Dictionary<string, string>
            {
                { "hashes", string.Join(",", TransactionModel.Transactions.Select(t => t.Hash).ToArray()) }
            };
            return await Provider.GetTransactions(100, 0, @params);
        }

        private async Task CopyToClipboard(string text)
        {
            await CopyService.CopyToClipboard(text);
        }
    }
}
