using Microsoft.AspNetCore.Components;
using static Mx.Blazor.DApp.Client.Application.Constants.MultiversxNetwork;
using static Mx.Blazor.DApp.Client.Application.Constants.DAppConstants;
using Mx.Blazor.DApp.Client.Models;
using Mx.NET.SDK.Domain.Data.Transactions;
using Mx.NET.SDK.Domain.Exceptions;

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
            if (!TransactionModel.Transactions.Select(t => t.Status == "pending").Any())
                return;

            CancellationToken cancellationToken = SyncToken.Token;
            await Task.Factory.StartNew(async () =>
            {
                for (int i = 0; i < TransactionModel.Transactions.Count; i++)
                {
                    if (TransactionModel.Transactions[i].Status != "pending")
                        continue;

                    try
                    {
                        var transaction = Transaction.From(TransactionModel.Transactions[i].Hash);
                        await transaction.AwaitExecuted(Provider, TxCheckTime);
                        TransactionModel.Transactions[i].Status = transaction.Status;
                    }
                    catch (TransactionException.TransactionStatusNotReachedException)
                    {
                        TransactionModel.Transactions[i].Status = "fail";
                    }
                    catch (TransactionException.TransactionWithSmartContractErrorException)
                    {
                        TransactionModel.Transactions[i].Status = "fail";
                    }
                    catch (TransactionException.FailedTransactionException)
                    {
                        TransactionModel.Transactions[i].Status = "fail";
                    }
                    catch (TransactionException.InvalidTransactionException)
                    {
                        TransactionModel.Transactions[i].Status = "invalid";
                    }
                    catch (Exception)
                    {
                        TransactionModel.Transactions[i].Status = "exception";
                    }
                    finally
                    {
                        StateHasChanged();
                        await Update.InvokeAsync(TransactionModel);
                    }
                }
                TransactionsContainer.TransactionsExecuted(TransactionModel.Transactions.Select(t => t.Hash).ToArray());
                StateHasChanged();

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

        private async Task CopyToClipboard(string text)
        {
            await CopyService.CopyToClipboard(text);
        }
    }
}
