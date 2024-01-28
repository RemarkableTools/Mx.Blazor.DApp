using Microsoft.AspNetCore.Components;
using static Mx.Blazor.DApp.Client.Application.Constants.MultiversxNetwork;
using static Mx.Blazor.DApp.Client.Application.Constants.DAppConstants;
using Mx.Blazor.DApp.Client.Models;
using Mx.NET.SDK.Provider.Dtos.API.Transactions;

namespace Mx.Blazor.DApp.Client.Shared.Components.Transactions
{
    public partial class AutomatedTx
    {
        [Parameter]
        public TransactionModel TransactionModel { get; set; } = default!;

        [Parameter]
        public EventCallback<TransactionModel> Update { get; set; }

        [Parameter]
        public EventCallback<TransactionModel> Dismiss { get; set; }

        private readonly CancellationTokenSource SyncToken = new();

        protected override async Task OnInitializedAsync()
        {
            CancellationToken cancellationToken = SyncToken.Token;
            await Task.Factory.StartNew(async () =>
            {
                if (!TransactionModel.Transactions.Any(t => t.Status == "pending")) // this is called only on page refresh
                    goto ALLEXECUTED;

                await Task.Delay(TxCheckTime); // delay before checking the transactions statuses

            GETTXS:
                var apiTransactions = await GetTransactions(); // get the transactions from API
                for (int i = 0; i < TransactionModel.Transactions.Count; i++) // set the status for all transactions in the model
                {
                    var transactionData = TransactionModel.Transactions[i];
                    var apiTransaction = apiTransactions.Where(t => t.TxHash == transactionData.Hash).SingleOrDefault();
                    if (apiTransaction != null)
                        transactionData.Status = apiTransaction.Status;
                    else
                        transactionData.Status = "invalid";
                }

                StateHasChanged(); // update UI for transactions in the model
                await Update.InvokeAsync(TransactionModel); // update stored transaction model

                if (TransactionModel.Transactions.Any(t => t.Status == "pending")) // there are still transactions pending
                {
                    await Task.Delay(TxCheckTime);
                    goto GETTXS;
                }

                TransactionsContainer.TransactionsExecuted(TransactionModel.Transactions.Select(t => t.Hash).ToArray());
                StateHasChanged();

            ALLEXECUTED:
                if (TX_DISMISS_TIME > 0)
                {
                    if (TransactionModel.Transactions.Find(tx => tx.Status != "success") == null)
                    {
                        await Task.Delay(TX_DISMISS_TIME);
                        await Dismiss.InvokeAsync(TransactionModel);
                    }
                }
            }, cancellationToken);
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
