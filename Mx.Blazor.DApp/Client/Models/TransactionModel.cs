namespace Mx.Blazor.DApp.Client.Models
{
    public class TransactionModel(string title, List<TransactionData> transactions)
    {
        public string Title { get; } = title;
        public List<TransactionData> Transactions { get; } = transactions;
    }
}
