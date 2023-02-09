using Mx.Blazor.DApp.Client.Models;

namespace Mx.Blazor.DApp.Client.Shared.Components.Transactions
{
    public partial class TransactionsPanel
    {
        protected override void OnInitialized()
        {
            TransactionsContainer.NewTxProcessing += NewTxEvent;
        }

        private void NewTxEvent()
        {
            StateHasChanged();
        }

        public void Update(TransactionModel transactionModel)
        {
            TransactionsContainer.UpdateTransaction(transactionModel);
        }

        public void Dismiss(TransactionModel transactionModel)
        {
            TransactionsContainer.RemoveTransaction(transactionModel);
        }
    }
}
