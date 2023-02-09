namespace Mx.Blazor.DApp.Client.Models
{
    public class TransactionModel
    {
        public string Title { get; set; }
        public List<TransactionData> Transactions { get; set; }

        public TransactionModel(string title, List<TransactionData> transactions)
        {
            Title = title;
            Transactions = transactions;
        }
    }
}
