namespace Mx.Blazor.DApp.Client.Models
{
    public class StorageTx(string title, List<TransactionData> txs, long timestamp)
    {
        public string Title { get; } = title;
        public List<TransactionData> Txs { get; set; } = txs;
        private long Timestamp { get; } = timestamp;

        public DateTime ToDate()
        {
            return DateTime.UnixEpoch.AddSeconds(Timestamp).ToUniversalTime();
        }
    }

    public class TransactionData(string hash, string status = "pending")
    {
        public string Hash { get; } = hash;
        public string Status { get; set; } = status;
    }
}
